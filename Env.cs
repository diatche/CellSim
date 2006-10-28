using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace AI
{
    public partial class Env : Form
    {
        internal Visual vis;
        internal Debug debug;
        internal Physics phy;
        internal F f;
        internal AI ai;
        public float timeInterval;
        public Random rn;
        public Point oldMouseLoc, oldMouseLoc2;
        bool maximised, dragging;
        public bool alive;
        DateTime oldMouseTime, oldMouseTime2;
        public int counter, rnC;
        int essence_spawn_time_rn;
        public float timeScale;
        public Size OldEnvSize;
        bool button_t, button_ctrl, button_shift;

        Thread thread;

        //settings
        float essence_spawn_time_mean = 1f;
        float essence_spawn_time_sd = 3f;

        public Env()
        {
            InitializeComponent();

            timeScale = 1f;
            UpdateTimeScale(0);
            rn = new Random((int)DateTime.Now.Ticks);
            OldEnvSize = this.Size;

            f = new F();
            debug = new Debug();

            Debug_Method();

            ai = new AI(this);
            phy = new Physics(this);
            vis = new Visual(this);

            maximised = false;
            alive = true;
            button_t = false;

            ThreadStart starter = delegate { timer_update(); };
            thread = new Thread(starter);
            thread.Start();

            this.DoubleBuffered = true;
        }

        private void UpdateTimeScale(float change)
        {
            float percent = 1 - change;
            timeScale *= percent;

            if (f != null)
            {
                timeScale = f.NumberBound(timeScale, 1f);
            }

            timeInterval = (float)timer.Interval / 1000 * timeScale;
        }

        private void Debug_Method()
        {
            //f.VectorPart(new PointF(0.0f, 19.62f), new PointF(-0.01171875f, -60.3446655f));

            //Console.WriteLine(Convert.ToInt32(true));
            //Console.WriteLine(Convert.ToInt32(false));

            //string s = portal.LoadData("tab.txt");

            //OleDbConnection cnADONetConnection = new OleDbConnection();
            //DateTime d = new DateTime();
            //d = d.AddDays(1);

            //float result;
            //float[] num = new float[6] {1,2,3,4,5,6};
            //result = f.SD(num);

            //float closest = f.Closest(new float[] { 1, 6, 9, 10 }, 5);

            //f.StudentDist_tValue(2.5f, 56);

            //float m1, m2;
            //f.StudentDist_CI(new float[10] { 17, 10, 9.75f, 9.5f, 12.5f, 10, 8.5f, 11.5f, 15, 11 }, 2.5f, out m1, out m2);

            //float z, x, c, v, b;
            //f.Summary(new float[5] { 2, 7, 1, -20, -4 }, out z, out x, out c, out v, out b);

            //double n = f.RoundToNearest(245.8, 0.7);

            //float[] n = f.RoundToNearest(new float[4] { 1.4f, 5.9f, 3.2f, 9.0f }, 0.5f);

            //float[,] n = f.Frequencies(new float[8] { 1.4f, 1.2f, 5.9f, 5.7f, 3.2f, 3.7f, 9.0f, 9.4f }, 1);

            //float[,] fg = new float[3, 2];
            //fg[0, 0] = 1;
            //fg[0, 1] = 4;
            //fg[1, 0] = 5;
            //fg[1, 1] = 2;
            //fg[2, 0] = 9;
            //fg[2, 1] = 1;

            //int nm = f.FrequencyMax(fg);

            //float n = 0.0f;
            //string s = n.ToString();

            //float[] n = f.FrequenciesToValues(new float[3] { 1, 2, 3 }, new float[] { 3, 3, 2 });

            //float n = f.RoundBySigFig(12.9f, 5);

            //double[] n = f.Sort(new double[5] { 7.9, 5.6, -1, 7.9, 9.9 }, true);
        }

        internal void ERROR(ErrorData errorData)
        {
            errorData.description = "ERROR: [Env]" + errorData.description;

            debug.NewError(errorData);

            //sub ERROR method example:
            //public void ERROR(ErrorData errorData)
            //{
            //    errorData.description = "[Database]" + errorData.description;
            //    env.ERROR(errorData);
            //}
        }

        void Env_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Point p = e.Location;

            if (e.Button == MouseButtons.Left)
            {
                if (vis.portal_select_being != null)
                {
                    vis.selected_being = vis.portal_select_being;
                    vis.portal_select_being = null;

                    vis.visPos = f.Add(f.Multiply(vis.selected_being.p, -vis.scale), new PointF(Width / 2, Height / 2));

                    return;
                }

                vis.Select(p, false);

                //    if (oldMouseLoc2 != oldMouseLoc)
                //{
                //    dragging = true;
                //}
            }
        }
        void Env_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Point p = e.Location;

            if (!dragging)
            {
                vis.Select(p, true);
            }

            if (e.Button == MouseButtons.Right)
            {
                if (vis.selected_hover_being != null)
                {
                    phy.DragTo(vis.selected_hover_being, vis.TransformToWorld(p));
                    dragging = true;
                }
                else if (vis.selected_hover_essence != null)
                {
                    phy.DragTo(vis.selected_hover_essence, vis.TransformToWorld(p));
                    dragging = true;
                }
                else
                {
                    //offset world
                    vis.DragScreen(f.Subtract(p, oldMouseLoc));
                }
            }

            oldMouseLoc2 = oldMouseLoc;
            oldMouseLoc = p;
            oldMouseTime2 = oldMouseTime;
            oldMouseTime = DateTime.Now;
        }
        void Env_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Point p = e.Location;

            if (e.Button == MouseButtons.Right)
            {
                if (dragging)
                {
                    TimeSpan time;
                    float s;

                    time = DateTime.Now - oldMouseTime2;
                    s = (float)f.TicksToSeconds(time.Ticks);
                    s = f.NumberBound(s, 0.015f);

                    if (vis.selected_hover_being != null)
                    {
                        phy.DragTo(vis.selected_hover_being, vis.TransformToWorld(oldMouseLoc2));
                        phy.DragTo(vis.selected_hover_being, vis.TransformToWorld(p), s);
                    }
                    else if (vis.selected_hover_essence != null)
                    {
                        phy.DragTo(vis.selected_hover_essence, vis.TransformToWorld(oldMouseLoc2));
                        phy.DragTo(vis.selected_hover_essence, vis.TransformToWorld(p), s);
                    }
                    dragging = false;
                }
            }
            else if (e.Button == MouseButtons.Left)
            {
                if (vis.selected_being != null & button_ctrl)
                {
                    vis.selected_being_follow = true;
                }
                else if (vis.selected_being == null & button_ctrl)
                {
                    vis.selected_being_follow = false;
                }
            }
        }
        void Env_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            float sensitivity;

            if (button_t)
            {
                sensitivity = -0.0002f;
                UpdateTimeScale((float)e.Delta * sensitivity);
            }
            else if (button_shift)
            {
                sensitivity = -0.01f;
                vis.ScrollStatus((int)((float)e.Delta * sensitivity));
            }
            else
            {
                sensitivity = -0.0002f;
                vis.ChangeScale((float)e.Delta * sensitivity);
            }
        }

        void Env_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                alive = !alive;
            }

            if (e.KeyCode == Keys.T)
            {
                button_t = true;
            }
            if (e.KeyCode == Keys.ControlKey)
            {
                button_ctrl = true;
            }
            if (e.KeyCode == Keys.ShiftKey)
            {
                button_shift = true;
            }
        }
        void Env_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            button_t = false;
            button_ctrl = false;
            button_shift = false;
        }

        void Env_ResizeBegin(object sender, System.EventArgs e)
        {
            timer.Enabled = false;
        }
        void Env_ResizeEnd(object sender, System.EventArgs e)
        {
            vis.ResisedEnv();
            timer.Enabled = true;

            OldEnvSize = this.Size;
        }

        void timer_update()
        {
            while (thread.IsAlive)
            {
                //restart
                if (ai.beings.Count == 0)
                {
                    ai.Restart();
                }

                //fix visPos
                if (!maximised)
                {
                    vis.ResisedEnv();
                    //vis.visPos = new PointF(Width, Height);
                    maximised = true;
                }

                if (alive)
                {
                    if (essence_spawn_time_rn <= 0)
                    {
                        essence_spawn_time_rn = (int)(f.NormDist_RandomX(essence_spawn_time_mean, essence_spawn_time_sd) / timeInterval);
                    }
                    else if (counter % essence_spawn_time_rn == 0)
                    {
                        if (ai.essences.Count == 0)
                        {
                            ai.NewEssence();
                        }
                        else if (ai.essences.Count == 1)
                        {
                            ai.NewEssence(ai.essences[0].p);
                        }
                        else
                        {
                            PointF p = new PointF();

                            if (ai.eatenEssence.Count != 0)
                            {
                                // spawn close to where the last few essences
                                // were eaten
                                int ran = rn.Next(0, ai.eatenEssence.Count);

                                p = ai.eatenEssence[ran];
                                ai.RemoveEatenEssence(p);
                            }
                            else if (ai.essences.Count != 0)
                            {
                                //spawn close to other essence (thats not taken)
                                int ran = rn.Next(0, ai.essences.Count);
                                Essence essence = ai.essences[ran];

                                while (ai.EssenceTaken(essence))
                                {
                                    ran = rn.Next(0, ai.essences.Count);
                                    essence = ai.essences[ran];
                                }

                                p = essence.p;
                            }
                            p = f.NormDist_RandomPoint(p, ai.spawn_spread);
                            ai.NewEssence(p);
                        }
                        essence_spawn_time_rn = (int)(f.NormDist_RandomX(essence_spawn_time_mean, essence_spawn_time_sd) / timeInterval);
                        vis.PlaySound_Essence_Add();
                    }

                    ai.Apply();
                    phy.Apply();
                }
                //vis.Draw();

                IncrementCounter();

                Thread.Sleep(timer.Interval);
            }
        }

        private void timer_Tick_1(object sender, EventArgs e)
        {

        }

        private void IncrementCounter()
        {
            if (counter == int.MaxValue)
            {
                counter = 0;
            }
            else
            {
                counter++;
            }
        }

        void Env_Disposed(object sender, System.EventArgs e)
        {
            vis.Dispose();
            ai.Dispose();
            thread.Abort();
            thread.Join();
        }
    }
}