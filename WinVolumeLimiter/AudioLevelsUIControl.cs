//Based on https://github.com/jeske/SoundLevelMonitor
using System;
using System.Collections.Generic;

using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace WinVolumeLimiter
{
    class AudioLevelsUIControl : Control
    {
        AudioLevelMonitor _audioMonitor;
        List<Pen> pens = new List<Pen>();
        Dictionary<string, Pen> _sessionIdToPen = new Dictionary<string, Pen>();
        Timer dispatcherTimer;
        Pen greenPen = new Pen(Brushes.Green, 0.5f);


        public AudioLevelsUIControl() {
            DoubleBuffered = true;
            dispatcherTimer = new Timer();
            dispatcherTimer.Tick += DispatcherTimer_Tick;
            dispatcherTimer.Interval = 100;            
            dispatcherTimer.Start();

            // populate pens                        
            pens.Add(new Pen(Brushes.Crimson, 1.0f));
            pens.Add(new Pen(Brushes.FloralWhite, 1.0f));
            pens.Add(new Pen(Brushes.HotPink, 1.0f));


            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e) {
            dispatcherTimer.Stop();
            this.Invalidate();
        }

        public AudioLevelMonitor AudioMonitor {
            get { return _audioMonitor; }
            set {
                _audioMonitor = value; 
                if (_audioMonitor != null)
                {
                    dispatcherTimer.Stop();
                    dispatcherTimer.Start();
                }
            }
        }

        private void RenderVUMeterGrid(Graphics g, double maxSample) {
            // make it look like a VU meter
            // g.FillRectangle(Brushes.Black,this.Bounds);   
            g.FillRectangle(Brushes.Black,0,0,Size.Width,Size.Height);

            double gridLine_step = 0.01;
            double gridLine_log = gridLine_step * 2;

            // draw gridlines every 0.1
            for (double x = 0.0; x < maxSample; x += gridLine_step) {
                int y = (int)(Size.Height - (Size.Height * (x / maxSample)));
                g.DrawLine(greenPen,
                    new Point(0, y),
                    new Point(Size.Width, y));

                // logarithmic gridlines
                if (x >= gridLine_log) {
                    gridLine_step *= 2;
                    gridLine_log = gridLine_step * 2;
                }
            }

        }

        /*
        // this feels expensive, but i'm not sure how else to do it
        private double computeMaxSampleLastN(AudioLevelMonitor.SampleInfo samples, int lastN) {
            double maxSample = 0.0;
            if(samples.samples.Length > 0)
                maxSample = samples.samples.Max();
            return maxSample;
        }*/

        private void DrawLines(Graphics g, double maxSample)
        {
            var monitorVol = AudioMonitor.MonitorVolume;
            Pen limitPen = pens[1];
            g.DrawLine(limitPen,
                new Point(0, (int)(Size.Height - (Size.Height * (monitorVol / maxSample)))),
                new Point(Size.Width, (int)(Size.Height - (Size.Height * (monitorVol / maxSample))))
            );

            var duckingVol = AudioMonitor.DuckingVolume;
            Pen duckingPen = pens[2];
            g.DrawLine(duckingPen,
                new Point(0, (int)(Size.Height - (Size.Height * (duckingVol / maxSample)))),
                new Point(Size.Width, (int)(Size.Height - (Size.Height * (duckingVol / maxSample))))
            );
        }
        protected override void OnPaint(PaintEventArgs pe) {
            base.OnPaint(pe);
            var g = pe.Graphics;

            double maxSample = 1.2f;


            // if we have no AudioMonitor draw a blank grid
            if (AudioMonitor == null) {
                RenderVUMeterGrid(g, 1.0);
                return;
            }
            // otherwise get samples, and draw a scaled rgid            
            var activeSamples = AudioMonitor.GetActiveSamples();
            maxSample = Math.Max(maxSample, 0.05); // make sure we don't divide by zero
            RenderVUMeterGrid(g, maxSample);


            Pen audioLevelPen = pens[0];
            string name = activeSamples.SessionName;
            double[] samples = activeSamples.samples;

            if (samples.Length > 0)
            {
                double last_sample = samples[samples.Length - 1];
                for (int x = 0; x < samples.Length - 1; x++)
                {
                    if (x > Size.Width)
                    {
                        break;
                    }
                    var sample = samples[samples.Length - (x + 1)];
                    g.DrawLine(audioLevelPen,
                        new Point(Size.Width - x, (int)(Size.Height - (Size.Height * (last_sample / maxSample)))),
                        new Point(Size.Width - (x + 1), (int)(Size.Height - (Size.Height * (sample / maxSample)))));
                    last_sample = sample;
                }
            }
            else
            {
                g.DrawLine(audioLevelPen,
                    new Point(Size.Width - 10, (int)(Size.Height - (Size.Height * (0 / maxSample)))),
                    new Point(Size.Width, (int)(Size.Height - (Size.Height * (0 / maxSample)))));
            }
            DrawLines(g, maxSample);
            dispatcherTimer.Start();
        }

    }
}
