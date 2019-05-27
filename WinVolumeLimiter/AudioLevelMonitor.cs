//Based on https://github.com/jeske/SoundLevelMonitor

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CSCore.CoreAudioAPI;

namespace WinVolumeLimiter
{
    public class AudioLevelMonitor
    {
        System.Timers.Timer sampleTimer;
        System.Timers.Timer restoreTimer;
        private int intervalms = 25;
        private Process process = null;

        public int Intervalms
        {
            get
            {
                return intervalms; 
            }
            set
            {
                intervalms = value;
                sampleTimer.Interval = intervalms;
            }
        }

        private int restoreDelay = 500;

        public int RestoreDelay
        {
            get { return restoreDelay; }
            set
            {
                restoreDelay = value;
                restoreTimer.Interval = restoreDelay;
            }
        }

        private SampleInfo info = new SampleInfo();
        IDictionary<string, List<double>> sessionIdToAudioSamples = new Dictionary<string, List<double>>();
        List<double> samples = new List<double>();
        int maxSamplesToKeep = 1000;

        private volatile float masterVolume = -1f;

        public float MasterVolume
        {
            get
            {
                return getMasterVolume();
            }
            set
            {
                masterVolume = value;
                setMasterVolume();
            }
        }

        public float MonitorVolume = .75f;
        public float DuckingVolume = .375f;
        private float volDiff;
        public bool Ducked = false;
        public bool DontChangeMax;

        public volatile string processName;
        private int pid;

        public AudioLevelMonitor(string ProcessName)
        {
            processName = ProcessName;
            initTimers();
        }

        public void Stop()
        {
            sampleTimer?.Stop();
            restoreTimer?.Stop();
            if(Ducked)
                RestoreTimer_Elapsed(null, null);
        }
        public void Start()
        {
            sampleTimer.Start();
            restoreTimer.Start();
        }

        public delegate void NewAudioSamplesEvent(AudioLevelMonitor monitor);
        public event NewAudioSamplesEvent NewAudioSamplesEventListeners;


        private void initTimers()
        {
            sampleTimer = new System.Timers.Timer(Intervalms);
            sampleTimer.Elapsed += SampleTimerElapsed;
            sampleTimer.AutoReset = false;
            sampleTimer.Start();

            restoreTimer = new System.Timers.Timer(RestoreDelay);
            restoreTimer.Elapsed += RestoreTimer_Elapsed;
            restoreTimer.AutoReset = false;
        }

        private void SampleTimerElapsed(object sender, System.Timers.ElapsedEventArgs e) {

            if (processName == "")
            {
                Thread.Sleep(1000);
                sampleTimer.Start(); // trigger next timer
                return;
            }

            if (process == null || ActiveSession == null)
            {
                ActiveSession = null;
                ActiveSession = getSession();
                if (ActiveSession == null)
                {
                    Thread.Sleep(1000); //We can do this since we're manually controlling the timer
                    sampleTimer.Start(); // trigger next timer
                    return;
                }
            }
            this.CheckAudioLevels();
            sampleTimer.Start(); // trigger next timer
        }
        private void truncateSamples(List<double> samples) {
            int excessSamples = samples.Count - maxSamplesToKeep;
            while (excessSamples-- > 0) {
                samples.RemoveAt(0);
            }
        }
        private bool areSamplesEmpty(List<double> samples) {
            foreach (var val in samples) {
                if (val != 0.0) {
                    return false;
                }
            }
            return true;
        }

        public struct SampleInfo
        {
            public string sessionId;
            public int pid;
            public string SessionName;
            public double[] samples;
        }


        public SampleInfo GetActiveSamples()
        {
            var info = new SampleInfo();
            lock (this)
            {
                info.samples = samples.ToArray();
            }
            return info;
        }

        public AudioSessionControl ActiveSession = null;

        private AudioSessionControl getSession()
        {
            lock (this)
            {
                try
                {
                    using (var sessionManager = GetDefaultAudioSessionManager2(DataFlow.Render))
                    {
                        using (var sessionEnumerator = sessionManager.GetSessionEnumerator())
                        {
                            foreach (var session in sessionEnumerator)
                            {
                                //This is stupid expensive
                                var audioSessionControl2 = session.QueryInterface<AudioSessionControl2>();
                                {
                                    using (var _process = audioSessionControl2.Process)
                                    {
                                        if (_process.ProcessName != processName)
                                        {
                                            continue;
                                        }

                                        string sessionid = audioSessionControl2.SessionIdentifier;
                                        pid = audioSessionControl2.ProcessID;
                                        if (process?.Id != pid || process?.ProcessName != audioSessionControl2.Process.ProcessName)
                                        {
                                            process = audioSessionControl2.Process;
                                            process.EnableRaisingEvents = true;
                                            process.Exited += (o, e) => process = null;
                                        }
                                        string name = audioSessionControl2.DisplayName;
                                        if (_process != null)
                                        {
                                            if (name == "") { name = _process.MainWindowTitle; }
                                            if (name == "") { name = _process.ProcessName; }
                                        }
                                        if (name == "") { name = "--unnamed--"; }

                                        var sessionInfo = new SampleInfo();
                                        sessionInfo.sessionId = sessionid;
                                        sessionInfo.pid = pid;
                                        sessionInfo.SessionName = name;

                                        info = sessionInfo;

                                        return session;
                                    }
                                }
                            }
                        }
                    }
                }
                catch (CoreAudioAPIException e)
                {
                    Console.WriteLine("AudioLevelMonitor exception: " + e.ToString());
                    return null;
                }
                return null;
            }
        }

        private volatile bool lastCheckOverMax = false;
        public void CheckAudioLevels() {
            volDiff = MonitorVolume - DuckingVolume;
            lock (this) {
                try
                {
                    var session = ActiveSession;
                    var peak = 0.0f;
                    using (var audioMeterInformation = session.QueryInterface<AudioMeterInformation>())
                    {
                        peak = audioMeterInformation.GetPeakValue();

                        samples.Add(peak);
                        truncateSamples(samples);
                        using (var simpleVolume = session.QueryInterface<SimpleAudioVolume>())
                        {
                            bool thisCheckOverMax = false;
                            float volume = simpleVolume.MasterVolume;

                            //If we're ducked, we guess what the actual volume would have been if we weren't ducked.
                            //The volume slider is "essentially linear" according to MS.
                            if (Ducked)
                            {
                                peak = peak + volDiff + .02f; //Add .02 to try and account for that "essentially" part
                            }
                                
                            if (peak > MonitorVolume)
                            {
                                thisCheckOverMax = true;
                            }

                            if (lastCheckOverMax && thisCheckOverMax)
                            {
                                 Ducked = true;
                                 simpleVolume.MasterVolume = DuckingVolume;
                            }
                            if (Ducked && thisCheckOverMax)
                            {
                                restoreTimer.Stop();
                                restoreTimer.Start();
                            }
                            lastCheckOverMax = true;
                        }
                    }
                } catch (CoreAudioAPIException e) {
                    Console.WriteLine("AudioLevelMonitor exception: " + e.ToString());
                    return;
                }
            } // lock
            System.GC.Collect();
            NewAudioSamplesEventListeners?.Invoke(this);
        }

        private void RestoreTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            lock (this)
            {
                try
                {
                    var session = ActiveSession;
                    using (var simpleVolume = session.QueryInterface<SimpleAudioVolume>())
                    {
                        var vol = simpleVolume.MasterVolume;
                        for (float i = vol; i <= MasterVolume; i += .01f)
                        {
                            simpleVolume.MasterVolume = i;
                        }
                        simpleVolume.MasterVolume = MasterVolume;

                        restoreTimer.Stop();
                        DontChangeMax = false;
                        Ducked = false;
                    }
                }
                catch (CoreAudioAPIException ex)
                {
                    Console.WriteLine("AudioLevelMonitor exception: " + ex.ToString());
                    return;
                }
            }
        }

        private static AudioSessionManager2 GetDefaultAudioSessionManager2(DataFlow dataFlow) {

            using (var enumerator = new MMDeviceEnumerator()) {
                using (var device = enumerator.GetDefaultAudioEndpoint(dataFlow, Role.Multimedia)) {
                    // Console.WriteLine("DefaultDevice: " + device.FriendlyName);
                    var sessionManager = AudioSessionManager2.FromMMDevice(device);
                    return sessionManager;
                }
            }
        }

        private float getMasterVolume()
        {
            if (masterVolume == -1f)
            {
                if (ActiveSession == null)
                {
                    //So this is stupid but we can't get a session on the main thread;
                    var t = new Thread(() =>
                    {
                        lock (this)
                        {
                            try
                            {
                                ActiveSession = getSession();
                                if (ActiveSession == null)
                                {
                                    masterVolume = 1.0f;
                                    return;
                                }
                                    
                                var session = ActiveSession;
                                using (var simpleVolume = session.QueryInterface<SimpleAudioVolume>())
                                {
                                    masterVolume = simpleVolume.MasterVolume;
                                }
                            }
                            catch (CoreAudioAPIException ex)
                            {
                                Console.WriteLine("AudioLevelMonitor exception: " + ex.ToString());
                            }
                        }
                    });
                    t.Start();
                    t.Join();
                }
            }
            return masterVolume;
        }
        private async void setMasterVolume()
        {
            if (ActiveSession == null)
                return;

            var t = new Thread(() =>
            {
                lock (this)
                {
                    try
                    {
                        var session = ActiveSession;
                        using (var simpleVolume = session.QueryInterface<SimpleAudioVolume>())
                        {
                            if (!Ducked)
                                simpleVolume.MasterVolume = MasterVolume;
                        }
                    }
                    catch (CoreAudioAPIException ex)
                    {
                        Console.WriteLine("AudioLevelMonitor exception: " + ex.ToString());
                        return;
                    }
                }
            });

            t.Start();
            t.Join();
        }
    }
}
