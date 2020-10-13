using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using Microsoft.DirectX.AudioVideoPlayback;
using System.Threading;

namespace AI
{
    class Visual
    {
        Env env;
        F f;
        public System.Windows.Forms.PaintEventArgs e;
        PointF INF_PointF;
        int counter;
        DateTime FPS_start;
        float FPS;
        public Being selected_being, selected_hover_being, portal_select_being;
        public Essence selected_hover_essence;
        Random rn;
        string status;
        public bool selected_being_follow;
        int status_miss_lines;

        Thread thread;

        //drawing
        Font font = new Font("Verdana", 8.25f, FontStyle.Regular);
        Pen pen, penThick, penThick2;
        SolidBrush brush_shadow_20, brush_shadow_50, brush_shadow_70;

        //sound
        Audio sound_ambience, sound_restart, sound_die, sound_born, sound_essence_add;
        Audio[] sound_eat;

        //settings
        public float scale = 1.0f;
        public PointF visPos;
        float tail_offset_coefficient = 1.4f;
        float tail_radius = 1.2f;

        public Visual(Env _env)
        {
            env = _env;
            f = env.f;

            INF_PointF = new PointF(float.PositiveInfinity, float.PositiveInfinity);
            FPS_start = DateTime.Now;

            rn = new Random((int)DateTime.Now.Ticks);
            status = "";
            selected_being_follow = false;

            visPos = new PointF(env.Width / 2, env.Height / 2);

            ThreadStart starter = delegate { Draw(); };
            thread = new Thread(starter);
            thread.Start();

            thread.Priority = ThreadPriority.BelowNormal;

            Load();
        }

        //||||| broken: does not sustain same coords when changing scale with
        //              visPos offset
        public void ChangeScale(float percentChange)
        {
            float percent = 1 - percentChange;

            PointF p = TransformToWorld(new PointF(env.Width / 2f, env.Height / 2f));
            PointF diff;

            //visPos = f.Divide(visPos, scale);

            scale = scale * percent;
            scale = f.NumberBound(scale, 0.1f, 1f);

            diff = f.Dir(TransformToScreen(p), new PointF(env.Width / 2f, env.Height / 2f));
            visPos = f.Add(visPos, diff);
        }

        public void Select(Point p, bool hover)
        {
            if (hover)
            {
                //hover select
                selected_hover_being = SelectBeing(p);
                selected_hover_essence = SelectEssence(p);
            }
            else
            {
                //select
                selected_being = SelectBeing(p);
            }
        }
        public Being SelectBeing(Point p)
        {
            int c = env.ai.beings.Count;
            if (c == 0)
            {
                return null;
            }

            p = TransformToWorld(p);

            ////check if inside first
            //for (int i = 0; i < c; i++)
            //{
            //    if (f.PointInsideRectangle(p, env.ai.beings[i].rectangle))
            //    {
            //        return env.ai.beings[i];
            //    }
            //}

            //check for closest
            float dist, closestDist, minDistMultiplier;
            Being closest, being;

            minDistMultiplier = 20.0f;

            closest = null;
            closestDist = float.MaxValue;

            try
            {
                IEnumerator<Being> en = env.ai.beings.GetEnumerator();

                while (en.MoveNext())
                {
                    being = en.Current;

                    dist = f.Distance(p, being.p);
                    if (dist < closestDist & dist < (float)Math.Pow(being.radius, 0.1) * minDistMultiplier)
                    {
                        closestDist = dist;
                        closest = being;
                    }
                }
                en.Dispose();
            }
            catch { }

            return closest;
        }
        public Essence SelectEssence(Point p)
        {
            int c = env.ai.essences.Count;
            if (c == 0)
            {
                return null;
            }

            p = TransformToWorld(p);

            //check for closest
            float dist, closestDist, minDistMultiplier;
            Essence closest, essence;

            minDistMultiplier = 4.0f;

            closest = null;
            closestDist = float.MaxValue;

            try
            {
                IEnumerator<Essence> en = env.ai.essences.GetEnumerator();

                while (en.MoveNext())
                {
                    essence = en.Current;

                    dist = f.Distance(p, essence.p);
                    if (essence.shell == null && dist < closestDist & dist < essence.radius * minDistMultiplier)
                    {
                        closestDist = dist;
                        closest = essence;
                    }
                }
                en.Dispose();
            }
            catch { }

            return closest;
        }

        public void DragScreen(Point screenOffset)
        {
            visPos = f.Add(visPos, screenOffset);
            selected_being_follow = false;
        }

        private void FollowSelectedBeing()
        {
            if (selected_being == null)
            {
                selected_being_follow = false;
            }

            if (selected_being_follow)
            {
                visPos = f.Add(f.Multiply(selected_being.p, -scale), new PointF(env.Width / 2, env.Height / 2));
            }
        }

        private void Draw()
        {
            while (thread.IsAlive)
            {
                //// check that all beings have updated
                //int c = env.ai.beings.Count;
                //if (c != 0)
                //{
                //    for (int i = 0; i < c; i++)
                //    {
                //        if (!env.ai.beings[i].updated)
                //        {
                //            return;
                //        }
                //    }
                //}

                env.Invalidate();

                GetFPS();
                IncrementCounter();

                Thread.Sleep(env.timer.Interval);
            }
        }

        void Update_e(object sender, System.Windows.Forms.PaintEventArgs newE)
        {
            e = newE;

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            portal_select_being = null;

            bool retry = true;

            while (retry)
            {
                retry = false;
                try
                {
                    e.Graphics.Transform = new Matrix(1, 0, 0, 1, 0, 0);

                    FollowSelectedBeing();

                    DrawStatus();
                    DrawRadar();

                    e.Graphics.Transform = new Matrix(scale, 0, 0, scale, visPos.X, visPos.Y);

                    DrawShell();
                    DrawEssence();
                    DrawBeing();
                }
                catch
                {
                    e.Graphics.Clear(env.BackColor);
                    retry = true;
                }
            }
        }

        public void ScrollStatus(int lines)
        {
            status_miss_lines += lines;

            status_miss_lines = f.NumberBound(status_miss_lines, 0);
        }

        private void DrawStatus()
        {
            if ((int)(0.1f / env.timeInterval) == 0 || counter % (int)(0.1f / env.timeInterval) == 0)
            {
                status = "";
            }

            if (status == "")
            {
                string str;

                str = "v" + env.debug.version + Environment.NewLine +
                    "Demonstrates simple AI. The beings need all of the different types of" + Environment.NewLine +
                "essence to survive." + Environment.NewLine +
                "- Full (non-roational) world movement implemented." + Environment.NewLine +
                "- Left-click to select, right-click to drag." + Environment.NewLine +
                "- Hold ctrl while left-clicking to lock camera onto target." + Environment.NewLine +
                "- To change the time scale, hold down 'T' and scroll" + Environment.NewLine +
                "  with the mouse wheel." + Environment.NewLine +
                "- You can 'jump to being' by clicking on its corresponding" + Environment.NewLine +
                "  waypoint in the radar (to the right)." + Environment.NewLine +
                "- Hold shift while scrolling whith the mouse wheel to" + Environment.NewLine +
                "  scroll this text." + Environment.NewLine +
                Environment.NewLine;

                str += "Scale: x" + string.Format("{0:0.00}", scale) + Environment.NewLine +
                    "Time Scale: x" + string.Format("{0:0.0}", env.timeScale) + Environment.NewLine;
                str += "Mouse: " + TransformToWorld(env.oldMouseLoc) + Environment.NewLine +
                    "Restarts: " + env.ai.restarts + Environment.NewLine +
                    "Age: " + f.ToString(new TimeSpan(f.SecondsToTicks(env.ai.age))) + Environment.NewLine +
                    Environment.NewLine;

                if (selected_being != null)
                {
                    str += "Selected Being:" + selected_being.ToString().Replace(Environment.NewLine, Environment.NewLine + "    ") + Environment.NewLine;
                }

                str += "Being averages:" + env.ai.Stats_Beings().Replace(Environment.NewLine, Environment.NewLine + "    ") + Environment.NewLine;
                str += "Essence averages:" + env.ai.Stats_Essence().Replace(Environment.NewLine, Environment.NewLine + "    ") + Environment.NewLine;

                //miss lines
                string[] lines;
                string[] delimit = new string[1];
                delimit[0] = Environment.NewLine;
                lines = str.Split(delimit, StringSplitOptions.RemoveEmptyEntries);

                status_miss_lines = f.NumberBound(status_miss_lines, 0, lines.Length - 1);

                int c = lines.Length;
                if (c != 0 & status_miss_lines != 0)
                {
                    for (int i = 0; i < status_miss_lines; i++)
                    {
                        str = str.Replace(lines[i] + Environment.NewLine, "");
                    }
                }

                //remove space
                while (str.Length != 0 && str.Substring(0, 2) == Environment.NewLine)
                {
                    str = str.Remove(0, 2);
                }

                if (status_miss_lines > 0)
                {
                    str = "...(" + status_miss_lines + " lines hidden. Hold down shift and scroll to change)..." + Environment.NewLine + str;
                }

                status = str;
            }


            DrawText(status, new Point());

            //FPS
            DrawText("FPS: " + string.Format("{0:0.0}", FPS), new Point(env.Width - 20, 0), Brushes.White, 1, 1);

            //paused + accuracy
            if (!env.alive)
            {
                DrawText("Paused", new Point(env.Width / 2, 0));
            }
            else if (env.timeScale > 1f)
            {
                DrawText("Accuracy: " + string.Format("{0:0}", 100f / Math.Pow(env.timeScale, 0.4)) + "%", new Point(env.Width / 2, 0));
            }
        }

        private void GetFPS()
        {
            int interval = 40;

            if (counter % interval == 0 && counter != 0)
            {
                TimeSpan time;
                float seconds;

                time = DateTime.Now - FPS_start;
                seconds = (float)f.TicksToSeconds(time.Ticks);

                FPS = interval / seconds;

                FPS_start = DateTime.Now;
            }
        }

        private void DrawRadar()
        {
            //shows in corner of screen in which
            //direction all of the beings are

            Point p, p1, p2, p3, v1, v2;
            PointF dir;
            PointF[] ps;
            Being being;

            float radius = 60f;

            p = new Point(env.Width - 90, 80);
            DrawCircle(p, radius, null, brush_shadow_70);
            DrawText("Radar", new Point(p.X, p.Y + (int)radius + 2), brush_shadow_70, 1, 0);


            int c = env.ai.beings.Count;
            if (c == 0)
            {
                return;
            }
            //for (int i = 0; i < c; i++)
            //{

            IEnumerator<Being> en = env.ai.beings.GetEnumerator();
            while (en.MoveNext())
            {
                being = en.Current;

                //p
                p1 = f.ConvertTo(TransformToScreen(being.p));

                dir = f.Dir(p, p1);
                if (dir == new PointF())
                {
                    dir = new PointF(0, 1);
                }

                p2 = f.ConvertTo(f.Offset(dir, radius - 5f));
                p3 = f.ConvertTo(f.Offset(dir, radius + 5f));

                p2 = f.Add(p2, p);
                p3 = f.Add(p3, p);

                DrawLine(p2, p3, Brushes.White);

                if (f.Distance(env.oldMouseLoc, f.MidPoint(p2, p3)) <= 10)
                {
                    //draw distance
                    DrawText(
                        "d: " + string.Format("{0:0}", f.Distance(TransformToWorld(p), being.p)) + Environment.NewLine +
                        "click to jump to"
                        , p, Brushes.White, 0, 0);
                    portal_select_being = being;
                }

                ////long term memory:
                //if (being.memory_essence.count_long != 0)
                //{
                //    ps = f.TangentsToCircleFromPoint(TransformToWorld(p), being.memory_essence.mean_long, being.memory_essence.sd_long * 3f);

                //    //p1 = f.ConvertTo(being.memory_essence.mean_long);
                //    //dir = f.Dir(TransformToWorld(p), p1);
                //    //if (dir == new PointF())
                //    //{
                //    //    dir = new PointF(0, 1);
                //    //}

                //    //p2 = f.ConvertTo(f.Add(p1, f.Offset(f.Perpendicular(dir, true), being.memory_essence.sd_long * 3f)));
                //    //p3 = f.ConvertTo(f.Add(p1, f.Offset(f.Perpendicular(dir, false), being.memory_essence.sd_long * 3f)));

                //    //left
                //    v1 = f.ConvertTo(TransformToScreen(ps[0]));
                //    dir = f.Dir(new PointF(p.X, p.Y), TransformToScreen(ps[0]));

                //    v1 = f.ConvertTo(f.Offset(dir, radius));
                //    v2 = f.ConvertTo(f.Offset(dir, radius + 5f));
                //    v1 = f.Add(v1, p);
                //    v2 = f.Add(v2, p);

                //    DrawLine(v1, v2, brush_shadow_50);

                //    //right
                //    v1 = f.ConvertTo(TransformToScreen(ps[1]));
                //    dir = f.Dir(new PointF(p.X, p.Y), TransformToScreen(ps[1]));

                //    v1 = f.ConvertTo(f.Offset(dir, radius));
                //    v2 = f.ConvertTo(f.Offset(dir, radius + 5f));
                //    v1 = f.Add(v1, p);
                //    v2 = f.Add(v2, p);

                //    DrawLine(v1, v2, brush_shadow_50);
                //}
            }
            en.Dispose();
        }

        private void DrawBeing()
        {
            IEnumerator<Being> en = env.ai.beings.GetEnumerator();

            while (en.MoveNext())
            {
                DrawBeing(en.Current);
            }
            en.Dispose();
        }
        private void DrawBeing(Being being)
        {
            Point p;

            //extra:
            if ((selected_being != null && being == selected_being) || (selected_hover_being != null && being == selected_hover_being))
            {
                ////info
                //p = new Point((int)(being.p.X + being.radius * 3f), (int)(being.p.Y + being.radius * 3f));

                //DrawText(being.ToString(), p);

                ////target
                //if (being.target_move != INF_PointF)
                //{
                //    DrawLine(being.p, being.target_move);
                //}

                ////selection
                //DrawCircle(being.p, being.radius * 2f , null, Brushes.LightSteelBlue);

                //visual radius
                DrawCircle(being.p, env.ai.being_lookAround_radius, null, brush_shadow_50);
                DrawText("Field of View", new Point((int)being.p.X, (int)(being.p.Y + env.ai.being_lookAround_radius) + 2), brush_shadow_50, 1, 0);

                //memory:
                if (being.memory_essence.count_short != 0)
                {
                    DrawCircle(being.memory_essence.mean_short, being.memory_essence.sd_short * 3, null, brush_shadow_50);
                    DrawText("Short-Term Memory", new Point((int)being.memory_essence.mean_short.X, (int)(being.memory_essence.mean_short.Y + being.memory_essence.sd_short * 3) + 2), brush_shadow_50, 1, 0);
                }
                if (being.memory_essence.count_long != 0)
                {
                    DrawCircle(being.memory_essence.mean_long, being.memory_essence.sd_long * 3, null, brush_shadow_20);
                    DrawText("Long-Term Memory", new Point((int)being.memory_essence.mean_long.X, (int)(being.memory_essence.mean_long.Y + being.memory_essence.sd_long * 3) + 2), brush_shadow_20, 1, 0);
                }
            }

            //glow
            DrawCircle(being.p, being.radius * 2.0f, brush_shadow_20, null);
            DrawCircle(being.p, being.radius * 1.5f, brush_shadow_50, null);
            //DrawCircle(being.p, being.radius * 0.5f, brush_shadow_70, null);

            //main:
            DrawCircle(being.p, being.radius);

            //dir
            float dist1, dist2, min, max;
            int alpha;
            PointF p1, p2, dir, dir2;

            f.MinMaxValue(being.amount, out min, out max);

            alpha = (int)(min * 255f);
            alpha = f.NumberBound(alpha, 0, 255);
            Color cl = Color.FromArgb(alpha, Color.White);
            SolidBrush brush = new SolidBrush(cl);

            if (being.target_move != INF_PointF)
            {
                dir = f.Dir(being.p, being.target_move);
            }
            else
            {
                if (being.dir != new PointF())
                {
                    dir = being.dir;
                }
                else
                {
                    dir = new PointF(0, -1);
                }
            }

            dist1 = being.radius * 1f;
            dist2 = being.radius * 0.5f;
            DrawCircle(being.p, dist2, brush, Brushes.White); //displays the state of the being

            p1 = f.Offset(dir, dist1);
            p1 = f.Add(p1, being.p);
            p2 = new PointF(); //f.Offset(dir, dist2);
            p2 = f.Add(p2, being.p);
            DrawLine(p1, p2);

            //mouth:
            Point[] ps = new Point[3];
            float closed;

            float bend = 0.7f;
            float size = 1.0f;

            closed = 1f - being.timer_feed / env.phy.being_feed_timer_start_s;

            dir = being.mouth_offset;
            dir = f.Offset(dir, being.radius * size);
            dir2 = f.Perpendicular(dir, false);

            for (int v = 0; v < 2; v++)
            {
                if (v == 1)
                {
                    dir2 = f.Opposite(dir2);
                }
                ps[0] = f.ConvertTo(being.mouth_offset);
                ps[1] = f.ConvertTo(f.Add(f.Add(ps[0], f.Multiply(dir2, bend)), f.Multiply(dir, (1f - bend))));
                ps[2] = f.ConvertTo(f.Add(f.Add(ps[0], f.Multiply(dir2, closed)), dir));
                for (int i = 0; i < 3; i++)
                {
                    ps[i] = f.Add(ps[i], f.ConvertTo(being.p));
                }

                e.Graphics.DrawCurve(pen, ps);
            }

            //animation:
            if (being.timer_creation > 0)
            {
                int x1, x2;
                x1 = (int)(env.ai.timer_creation / env.timeInterval);
                x1 = f.NumberBound(x1, 1);
                x2 = (int)(being.timer_creation / env.timeInterval);
                x2 = f.NumberBound(x2, 1);
                DrawRipple(being.p, being.radius * 10f, being.radius, x1, x2, brush_shadow_70);
                being.timer_creation -= env.timeInterval;
            }

            //tail:
            dir = f.Opposite(being.velocity);
            if (dir == new PointF())
            {
                dir = new PointF(0, 1);
            }
            dir = f.Offset(dir, being.radius * tail_offset_coefficient);

            p = f.Ceiling(f.Add(being.p, dir));

            DrawCircle(p, tail_radius, null, Brushes.White);

            //body:
            float dist_coefficient = 1.8f;

            if (being.shells.Count == 0)
            {
                DrawCircle(being.p, being.radius * dist_coefficient, null, brush_shadow_70);
            }
            else
            {
                PointF dir1, dir3;
                Shell shell;
                int index;

                int c = being.shells.Count;
                ps = new Point[c * 2 + 4];
                dir1 = new PointF();

                for (int i = 0; i < c; i++)
                {
                    index = i + 0;

                    //left side
                    shell = being.shells[i];

                    if (i != 0)
                    {
                        dir1 = f.Dir(shell.p, being.shells[i - 1].p);
                    }
                    else
                    {
                        dir1 = f.Dir(shell.p, being.p);
                    }

                    if (dir1 == new PointF())
                    {
                        dir1 = new PointF(0, 1);
                    }
                    dir1 = f.Perpendicular(dir1, true);
                    dir1 = f.Offset(dir1, shell.radius * dist_coefficient);

                    ps[index] = f.ConvertTo(f.Add(shell.p, dir1));

                    //right side
                    dir2 = f.Opposite(dir1);

                    ps[c * 2 - index] = f.ConvertTo(f.Add(shell.p, dir2));

                    //end
                    if (i == c - 1)
                    {
                        dir1 = f.Perpendicular(dir1, true);
                        ps[c * 2 - index - 1] = f.ConvertTo(f.Add(shell.p, dir1));
                    }
                }

                //head
                //starting from right side
                if (being.shells.Count != 0 && being.shells[0] != null)
                {
                    dir1 = f.Dir(being.shells[0].p, being.p);
                }
                if (dir1 == new PointF())
                {
                    dir1 = new PointF(0, 1);
                }
                dir1 = f.Perpendicular(dir1, false);
                dir1 = f.Offset(dir1, being.radius * dist_coefficient);
                dir2 = f.Perpendicular(dir1, true);
                dir3 = f.Perpendicular(dir2, true);
                ps[ps.Length - 3] = f.ConvertTo(f.Add(being.p, dir1));
                ps[ps.Length - 2] = f.ConvertTo(f.Add(being.p, dir2));
                ps[ps.Length - 1] = f.ConvertTo(f.Add(being.p, dir3));

                ////check that no point is close to another
                //float min_distance = 5f;

                //c = ps.Length;
                //for (int i = 0; i < c; i++)
                //{
                //    for (int v = 0; v < c; v++)
                //    {
                //        if (i != v && f.Distance(ps[i], ps[v]) < min_distance)
                //        {
                //            ps[i] = Point.Empty;
                //        }
                //    }
                //}
                //ps = f.Clean(ps);

                ////check that no point is inside shell and being
                //int cc;

                //c = ps.Length;
                //for (int i = 0; i < c; i++)
                //{
                //    cc = being.shells.Count;
                //    for (int v = 0; v < cc; v++)
                //    {
                //        if (f.Distance(ps[i], f.ConvertTo(being.shells[v].p)) < being.shells[v].radius)
                //        {
                //            ps[i] = Point.Empty;
                //        }
                //    }
                //}
                //ps = f.Clean(ps);

                e.Graphics.DrawClosedCurve(new Pen(brush_shadow_70), ps);
            }

            being.updated = false;
        }

        private void DrawShell()
        {
            IEnumerator<Shell> en = env.ai.shells.GetEnumerator();

            while (en.MoveNext())
            {
                DrawShell(en.Current);
            }
            en.Dispose();
        }
        private void DrawShell(Shell shell)
        {
            ////extra:
            //if ((selected_being != null && being == selected_being) || (selected_hover_being != null && being == selected_hover_being))
            //{
            //    ////target
            //    //if (being.target_move != INF_PointF)
            //    //{
            //    //    DrawLine(being.p, being.target_move);
            //    //}

            //    //selection
            //    DrawCircle(being.p, being.radius * 2f);
            //}

            //glow
            DrawCircle(shell.p, shell.radius * 2.0f, brush_shadow_20, null);
            DrawCircle(shell.p, shell.radius * 1.5f, brush_shadow_50, null);

            //main:
            Point p;
            PointF dir;

            //hollow circle
            DrawCircle(shell.p, shell.radius, null, brush_shadow_70);

            //tail:
            dir = f.Opposite(shell.velocity);
            if (dir == new PointF())
            {
                dir = new PointF(0, -1);
            }
            dir = f.Offset(dir, shell.radius);
            dir = f.Multiply(dir, tail_offset_coefficient);

            p = f.Ceiling(f.Add(shell.p, dir));

            try
            {
                DrawCircle(p, tail_radius, null, Brushes.White);
            }
            catch { }
        }

        private void DrawEssence()
        {
            IEnumerator<Essence> en = env.ai.essences.GetEnumerator();

            while (en.MoveNext())
            {
                DrawEssence(en.Current);
            }
            en.Dispose();
        }
        private void DrawEssence(Essence essence)
        {
            Point p;

            //extra:
            if (selected_hover_essence != null && essence == selected_hover_essence)
            {
                //info
                p = new Point((int)(essence.p.X + essence.radius * 1.5f), (int)(essence.p.Y + essence.radius * 1.5f));

                DrawText(essence.ToString(), p, brush_shadow_70);
            }

            //animation:
            if (essence.timer_creation > 0)
            {
                int x1, x2;
                x1 = (int)(env.ai.timer_creation / env.timeInterval);
                x1 = f.NumberBound(x1, 1);
                x2 = (int)(essence.timer_creation / env.timeInterval);
                x2 = f.NumberBound(x2, 1);
                DrawRipple(essence.p, essence.radius * 10f, essence.radius, x1, x2, brush_shadow_70);
                essence.timer_creation -= env.timeInterval;
            }

            //glow
            if (essence.shell == null)
            {
                DrawCircle(essence.p, essence.radius * 2.0f, brush_shadow_20, null);
                DrawCircle(essence.p, essence.radius * 1.5f, brush_shadow_50, null);
            }

            switch (essence.type)
            {
                case 0:
                    //protein
                    //main:

                    //Point p;
                    //PointF dir;

                    //hollow circle
                    DrawCircle(essence.p, essence.radius);

                    ////draw tail if no shell:
                    //if (essence.shell == null)
                    //{
                    //    dir = f.Opposite(essence.velocity);
                    //    if (dir == new PointF())
                    //    {
                    //        dir = new PointF(0, -1);
                    //    }
                    //    dir = f.Offset(dir, essence.radius);
                    //    dir = f.Multiply(dir, tail_offset_coefficient);

                    //    p = f.Ceiling(f.Add(essence.p, dir));

                    //    DrawCircle(p, tail_radius, null, Brushes.White);
                    //}
                    break;
                case 1:
                    //carbo
                    //3 swirling circles
                    //main:

                    float radius = 1.5f;
                    float speed = 0.1f;

                    Point p1, p2, p3;

                    p1 = new Point();
                    p2 = new Point();
                    p3 = new Point();

                    p1.X = (int)(Math.Cos(env.counter * speed) * essence.radius);
                    p1.Y = (int)(Math.Sin(env.counter * speed) * essence.radius);

                    p2.X = (int)(Math.Cos(env.counter * speed + 1.3f / 2f * Math.PI) * essence.radius);
                    p2.Y = (int)(Math.Sin(env.counter * speed + 1.3f / 2f * Math.PI) * essence.radius);

                    p3.X = (int)(Math.Cos(env.counter * speed + 2.7f / 2f * Math.PI) * essence.radius);
                    p3.Y = (int)(Math.Sin(env.counter * speed + 2.7f / 2f * Math.PI) * essence.radius);

                    p1 = f.Add(p1, f.ConvertTo(essence.p));
                    p2 = f.Add(p2, f.ConvertTo(essence.p));
                    p3 = f.Add(p3, f.ConvertTo(essence.p));

                    DrawCircle(p1, radius, null, Brushes.White);
                    DrawCircle(p2, radius, null, Brushes.White);
                    DrawCircle(p3, radius, null, Brushes.White);

                    break;
            }
        }

        private void DrawLine(PointF p1, PointF p2)
        {
            e.Graphics.DrawLine(pen, p1, p2);
        }
        private void DrawLine(PointF p1, PointF p2, Brush brush)
        {
            e.Graphics.DrawLine(new Pen(brush), p1, p2);
        }
        private void DrawLine_Hollow(PointF p1, PointF p2, float width, Brush fill)
        {
            //work out direction. take perpendiculars on both sides
            //and using the width, work out the 4 points to make the
            //rectangle.

            PointF dir, perp1, perp2;
            PointF[] points = new PointF[4];

            dir = f.Subtract(p2, p1);

            perp1 = f.Perpendicular(dir, true);
            perp1 = f.Offset(perp1, width / 2f);
            perp2 = f.Opposite(perp1);

            //p1
            points[0] = f.Add(p1, perp1);
            points[1] = f.Add(p1, perp2);

            //p2
            points[2] = f.Add(p2, perp2);
            points[3] = f.Add(p2, perp1);

            if (fill != null)
            {
                e.Graphics.FillPolygon(fill, points);
            }
            e.Graphics.DrawPolygon(pen, points);
        }

        private void DrawDot(Pen _pen, PointF p)
        {
            e.Graphics.DrawLine(_pen, new PointF(p.X - _pen.Width / 2, p.Y), new PointF(p.X + _pen.Width / 2, p.Y));
        }

        private void DrawDiamond(Pen pen, PointF p1, PointF p2, float width, Brush fill)
        {
            //work out direction. take perpendiculars on both sides
            //and using the width, work out the 2 other points to make the
            //diamond.

            PointF dir, perp1, perp2, mp;
            PointF[] points = new PointF[4];

            dir = f.Subtract(p2, p1);
            mp = f.MidPoint(p1, p2);

            perp1 = f.Perpendicular(dir, true);
            perp1 = f.Offset(perp1, width / 2f);
            perp2 = f.Opposite(perp1);

            //p1
            points[0] = p1;
            points[1] = f.Add(mp, perp2);

            //p2
            points[2] = p2;
            points[3] = f.Add(mp, perp1);

            if (fill != null)
            {
                e.Graphics.FillPolygon(fill, points);
            }
            e.Graphics.DrawPolygon(pen, points);
        }

        /// <summary>
        /// Creates a new visual item.
        /// </summary>
        /// <param name="handle">The data object that this item handles.</param>
        /// <param name="rectangle">The position and size of this item.</param>
        /// <param name="type">
        /// 0 = treeBox
        /// 1 = treeItem
        /// </param>
        //private VisItem NewItem(object handle, Rectangle rectangle, int type)
        //{
        //    VisItem item = new VisItem(handle, rectangle, type);
        //    items.Add(item);

        //    return item;
        //}

        //private void DrawTree(Point loc)
        //{
        //    int c = categoryTree.Count;
        //    if (c == 0)
        //    {
        //        return;
        //    }

        //    Category category;
        //    Rectangle rect;
        //    Point p;
        //    int boxHeight = font.Height / 2;

        //    p = loc;

        //    int offset = 10; //offset

        //    for (int i = 0; i < c; i++)
        //    {
        //        category = categoryTree[i];
        //        p.X = category.level * offset + loc.X;

        //        if (category.hasChild)
        //        {
        //            rect = DrawTreeBox(p, boxHeight, category.expanded);
        //            NewItem(category, rect, 0);

        //            p.X += rect.Width * 2;
        //        }

        //        if (category == selectedCategory)
        //        {
        //            rect = DrawText(category.name, p, Brushes.Gray);
        //            NewItem(category, rect, 1);
        //        }
        //        else
        //        {
        //            rect = DrawText(category.name, p, 1, -1);
        //            NewItem(category, rect, 1);
        //        }
        //        p.Y += font.Height;
        //    }
        //}

        private void DrawRipple(PointF p, float radius1, float radius2, int steps, int currentStep, Brush brush)
        {
            float radius, stepLength;
            stepLength = (radius2 - radius1) / (float)steps;

            radius = radius1 + stepLength * currentStep;

            DrawCircle(p, radius, null, brush);
        }

        private void DrawCircle(PointF p, float radius)
        {
            DrawCircle(p, radius, null);
        }
        private void DrawCircle(PointF p, float radius, Brush fill)
        {
            DrawCircle(p, radius, fill, Brushes.White);
        }
        private void DrawCircle(PointF p, float radius, Brush fill, Brush line)
        {
            if (radius == 1)
            {
                e.Graphics.DrawLine(pen, p, new PointF(p.X + 1f, p.Y));
                return;
            }

            p.X -= radius;
            p.Y -= radius;

            Rectangle r = new Rectangle(new Point((int)p.X, (int)p.Y), new Size((int)(radius * 2f), (int)(radius * 2f)));

            if (fill != null)
            {
                e.Graphics.FillEllipse(fill, r);
            }
            if (line != null)
            {
                e.Graphics.DrawEllipse(new Pen(line), r);
            }
        }

        private void DrawText(string text, Point p)
        {
            e.Graphics.DrawString(text, font, Brushes.White, p);
        }
        /// <summary>
        /// Draws text with specified alignment.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="p"></param>
        /// <param name="verticalAlignment">-1: bottom, 0: center, 1: top</param>
        /// <param name="horizontalAlignment">-1: left, 0: center, 1: right</param>
        /// <returns></returns>
        private Rectangle DrawText(string text, Point p, Brush brush, int verticalAlignment, int horizontalAlignment)
        {
            int width, height;
            Rectangle rectangle = new Rectangle();
            rectangle.Location = p;

            verticalAlignment--;
            horizontalAlignment++;

            //vertical
            height = font.Height;
            p.Y += height / 2 * verticalAlignment;

            //horizontal
            width = (int)e.Graphics.MeasureString(text, font).Width;
            p.X -= width / 2 * horizontalAlignment;

            DrawText(text, p, brush);

            rectangle.Width = width;
            rectangle.Height = height;

            return rectangle;
        }
        private Rectangle DrawText(string text, Point p, Brush brush, Brush highlight)
        {
            Rectangle r = new Rectangle();

            if (highlight != null)
            {
                r = new Rectangle(p, new Size(f.MeasureString(text, font, e) + 2, font.Height));
                e.Graphics.FillRectangle(highlight, r);
            }
            e.Graphics.DrawString(text, font, brush, p);

            return r;
        }
        private void DrawText(string text, Point p, Brush brush)
        {
            e.Graphics.DrawString(text, font, brush, p);
        }

        public Point TransformToWorld(Point p)
        {
            //transform point
            Matrix m = new Matrix(scale, 0, 0, scale, visPos.X, visPos.Y);
            m.Invert();

            Point[] ps = new Point[1];
            ps[0] = p;
            m.TransformPoints(ps);

            p = ps[0];

            return p;
        }
        public PointF TransformToWorld(PointF p)
        {
            //transform point
            Matrix m = new Matrix(scale, 0, 0, scale, visPos.X, visPos.Y);
            m.Invert();

            PointF[] ps = new PointF[1];
            ps[0] = p;
            m.TransformPoints(ps);

            p = ps[0];

            return p;
        }
        public Point TransformToScreen(Point p)
        {
            //transform point
            Matrix m = new Matrix(scale, 0, 0, scale, visPos.X, visPos.Y);

            Point[] ps = new Point[1];
            ps[0] = p;
            m.TransformPoints(ps);

            p = ps[0];

            return p;
        }
        public PointF TransformToScreen(PointF p)
        {
            //transform point
            Matrix m = new Matrix(scale, 0, 0, scale, visPos.X, visPos.Y);

            PointF[] ps = new PointF[1];
            ps[0] = p;
            m.TransformPoints(ps);

            p = ps[0];

            return p;
        }

        private void Load()
        {
            env.Paint += new System.Windows.Forms.PaintEventHandler(Update_e);

            pen = new Pen(Brushes.White);
            penThick = new Pen(Brushes.White, 2);
            penThick2 = new Pen(Brushes.White, 4);

            // Create a custom brush using a semi-transparent color
            Color customColor1 = Color.FromArgb(50, Color.Azure);
            Color customColor2 = Color.FromArgb(70, Color.Azure);
            Color customColor3 = Color.FromArgb(20, Color.Azure);
            brush_shadow_20 = new SolidBrush(customColor3);
            brush_shadow_50 = new SolidBrush(customColor1);
            brush_shadow_70 = new SolidBrush(customColor2);


            //sound:
            //ambience
            sound_ambience = new Audio(CleanPath("pad.wav"));
            sound_ambience.Play();
            //ambience.Volume = 0;
            //ambience.Stopping += new EventHandler(ambience_Stopping);
            sound_ambience.SeekStopPosition(sound_ambience.Duration - 2.1, SeekPositionFlags.RelativePositioning);
            sound_ambience.Ending += new EventHandler(ambience_Ending);
            sound_ambience.Volume = -1200;

            //eat sound
            sound_eat = new Audio[2];
            int c = sound_eat.Length;
            for (int i = 0; i < c; i++)
            {
                sound_eat[i] = new Audio(CleanPath("eat_" + i + ".wav"), false);
                sound_eat[i].Volume = 0;
                //sound_eat[i].SeekStopPosition(2, SeekPositionFlags.RelativePositioning);
            }

            //restart
            sound_restart = new Audio(CleanPath("restart.wav"));
            sound_restart.Volume = -1200;

            //born
            sound_born = new Audio(CleanPath("born.wav"));
            sound_born.Volume = -2000;

            //die
            sound_die = new Audio(CleanPath("die.WAV"));
            sound_die.Volume = -1500;

            //sound_essence_add
            sound_essence_add = new Audio(CleanPath("essence_add.wav"));
            sound_essence_add.Volume = -3500;
        }

        public void PlaySound_Being_Eat()
        {
            int i = rn.Next(0, sound_eat.Length);

            if (sound_eat[i].Disposed)
            {
                return;
            }

            sound_eat[i].CurrentPosition = 0;

            if (!sound_eat[i].Playing)
            {
                sound_eat[i].Play();
            }
        }
        public void PlaySound_Being_Born()
        {
            if (sound_born.Disposed)
            {
                return;
            }

            sound_born.CurrentPosition = 0;

            if (!sound_born.Playing)
            {
                sound_born.Play();
            }
        }
        public void PlaySound_Being_Die()
        {
            if (sound_die.Disposed)
            {
                return;
            }

            sound_die.CurrentPosition = 0;

            if (!sound_die.Playing)
            {
                sound_die.Play();
            }
        }
        public void PlaySound_Restart()
        {
            if (sound_born.Disposed)
            {
                return;
            }

            sound_born.CurrentPosition = 0;

            if (!sound_born.Playing)
            {
                sound_born.Play();
            }
        }
        public void PlaySound_Essence_Add()
        {
            //if (sound_essence_add.Disposed)
            //{
            //    return;
            //}

            //sound_essence_add.CurrentPosition = 0;

            //if (!sound_essence_add.Playing)
            //{
            //    sound_essence_add.Play();
            //}
        }

        void ambience_Ending(object sender, EventArgs e)
        {
            sound_ambience.Play();
            sound_ambience.CurrentPosition = 0.2;
        }

        public void ResisedEnv()
        {
            //visPos = f.Add(f.Multiply(visPos, scale), new PointF(env.Width / 2, env.Height / 2));

            visPos = new PointF(visPos.X * ((float)env.Width / (float)env.OldEnvSize.Width),
                                visPos.Y * ((float)env.Height / (float)env.OldEnvSize.Height));
        }

        public void Reload()
        {
            Load();
        }

        public void Dispose()
        {
            brush_shadow_20.Dispose();
            brush_shadow_50.Dispose();
            brush_shadow_70.Dispose();

            sound_ambience.Dispose();
            sound_born.Dispose();
            sound_die.Dispose();
            sound_essence_add.Dispose();
            sound_restart.Dispose();

            int c = sound_eat.Length;
            for (int i = 0; i < c; i++)
            {
                sound_eat[i].Dispose();
            }

            thread.Abort();
            thread.Join();
        }

        private string CleanPath(string path)
        {
            if (path.Substring(1, 1) != ":")
            {
                path = System.Windows.Forms.Application.StartupPath + "\\res\\" + path;
            }
            return path;
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

        private void ERROR(ErrorData errorData)
        {
            errorData.description = "[Visual]" + errorData.description;
            env.ERROR(errorData);
        }
    }
}
