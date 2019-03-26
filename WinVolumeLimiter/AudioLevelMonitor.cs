//Based on https://github.com/jeske/SoundLevelMonitor

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using CSCore.CoreAudioAPI;

namespace WinVolumeLimiter
{
    public class AudioLevelMonitor
    {
        System.Timers.Timer dispatchingTimer;
        System.Timers.Timer restoreTimer;
        public int interval_ms { get; set; } = 20;
        public int restoreDelay { get; set; } = 200;
        private SampleInfo info = new SampleInfo();
        IDictionary<string, List<double>> sessionIdToAudioSamples = new Dictionary<string, List<double>>();
        List<double> samples = new List<double>();
        int maxSamplesToKeep = 1000;
        private volatile float oldVolume = 0f;
        public float MaxVolume = 1.0f;
        public bool Ducked = false;
        public bool DontChangeMax;
        
        public string processName { get; set; } = String.Empty;
        private int pid;

        public AudioLevelMonitor()
        {
            dispatchingTimer = new System.Timers.Timer(interval_ms);
            dispatchingTimer.Elapsed += DispatchingTimer_Elapsed;
            dispatchingTimer.AutoReset = false;
            dispatchingTimer.Start();

            restoreTimer = new System.Timers.Timer(restoreDelay);
            restoreTimer.Elapsed += RestoreTimer_Elapsed;
            restoreTimer.AutoReset = false;
        }
        public void Stop() {
            dispatchingTimer.Stop();
        }

        public delegate void NewAudioSamplesEvent(AudioLevelMonitor monitor);
        public event NewAudioSamplesEvent NewAudioSamplesEventListeners;

        private void DispatchingTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {

            if (processName == "")
            {
                Thread.Sleep(1000);
                dispatchingTimer.Start(); // trigger next timer
                return;
            }


            if (Process.GetProcesses().Any(x => x.Id == pid && x.ProcessName == processName) == false || ActiveSession == null)
            {
                ActiveSession = null;
                ActiveSession = getSession();
                if (ActiveSession == null)
                {
                    Thread.Sleep(1000);
                    dispatchingTimer.Start(); // trigger next timer
                    return;
                }
            }
            this.CheckAudioLevels();
            dispatchingTimer.Start(); // trigger next timer
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
                                    using (var process = audioSessionControl2.Process)
                                    {
                                        if (process.ProcessName != processName)
                                        {
                                            continue;
                                        }

                                        string sessionid = audioSessionControl2.SessionIdentifier;
                                        pid = audioSessionControl2.ProcessID;
                                        string name = audioSessionControl2.DisplayName;
                                        if (process != null)
                                        {
                                            if (name == "") { name = process.MainWindowTitle; }
                                            if (name == "") { name = process.ProcessName; }
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

        public void CheckAudioLevels() {
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
                            float volume = simpleVolume.MasterVolume;
                            if (peak > MaxVolume && simpleVolume.MasterVolume >= MaxVolume)
                            {
                                if (!Ducked || DontChangeMax)
                                {
                                    if (!DontChangeMax)
                                    {
                                        oldVolume = volume;
                                        DontChangeMax = false;
                                    }
                                    Ducked = true;
                                    simpleVolume.MasterVolume = MaxVolume;
                                }
                                restoreTimer.Stop();
                                restoreTimer.Start();
                            }
                        }
                    }
                } catch (CoreAudioAPIException e) {
                    Console.WriteLine("AudioLevelMonitor exception: " + e.ToString());
                    return;
                }

                // before we are done, we need to add samples to anyone we didn't see
                /*
                var deleteSamplesForPids = new HashSet<string>();
                foreach (var kvp in sessionIdToAudioSamples) {
                    if (!seenPids.Contains(kvp.Key)) {
                        kvp.Value.Add(0.0);
                        truncateSamples(kvp.Value);
                        if (areSamplesEmpty(kvp.Value)) {
                            deleteSamplesForPids.Add(kvp.Key);
                        }
                    }
                }
                foreach (var sessionid in deleteSamplesForPids) {
                    sessionIdToAudioSamples.Remove(sessionid);
                }*/
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
                        for (float i = vol; i < oldVolume; i += .01f)
                        {
                            simpleVolume.MasterVolume = i;
                        }

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

    }
}
