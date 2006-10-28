using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace AI
{
    class Physics
    {
        Env env;
        F f;
        AI ai;
        PointF force_current;
        PointF current_direction;
        public float frictionPerTick, current_magnitude;
        PointF INF_PointF;
        Random rn;

        //settings
        float friction = 0.5f;
        float being_feed_radius_coefficient = 0.8f;
        public float being_feed_timer_start_s = 1f;
        float current_magnitude_sd = 10f;
        float current_direction_sd = 2f;
        public float essence_movement_force_sd = 700f;
        int essence_update_interval = 1;

        public Physics(Env _env)
        {
            env = _env;
            f = env.f;
            ai = env.ai;

            INF_PointF = new PointF(float.PositiveInfinity, float.PositiveInfinity);

            rn = new Random((int)DateTime.Now.Ticks);
        }

        public void DragTo(Being being, Point p)
        {
            if (being == null)
            {
                return;
            }

            //reset everything
            being.force = new PointF();
            being.velocity = new PointF();

            being.p = p;
            CorrectDistance();
        }
        public void DragTo(Being being, Point p, float seconds)
        {
            if (being == null)
            {
                return;
            }

            //reset everything
            being.force = new PointF();
            being.velocity = new PointF();

            PointF dir, dist, newP, vChange;

            newP = p;
            dir = f.Dir(being.p, p);
            dist = dir;

            //realise distance
            being.p = newP;

            //update velocity:
            //change in v = d/t
            vChange = f.Divide(dist, seconds);
            being.velocity = f.Add(being.velocity, vChange);
            being.velocity = f.Multiply(being.velocity, friction);

            CorrectDistance();
        }
        public void DragTo(Essence essence, Point p)
        {
            if (essence == null)
            {
                return;
            }

            //reset everything
            essence.force = new PointF();
            essence.velocity = new PointF();

            essence.p = p;
        }
        public void DragTo(Essence essence, Point p, float seconds)
        {
            if (essence == null)
            {
                return;
            }

            //reset everything
            essence.force = new PointF();
            essence.velocity = new PointF();

            PointF dir, dist, newP, vChange;

            newP = p;
            dir = f.Dir(essence.p, p);
            dist = dir;

            //realise distance
            essence.p = newP;

            //update velocity:
            //change in v = d/t
            vChange = f.Divide(dist, seconds);
            essence.velocity = f.Add(essence.velocity, vChange);
            essence.velocity = f.Multiply(essence.velocity, friction);

            CorrectDistance();
        }

        public void Apply()
        {
            frictionPerTick = 1f - (friction * env.timeInterval);

            ////ApplyCurrent();
            //RandomEssenceMovement();

            //RealiseForce();

            ////CheckBounds();
            //CorrectDistance();

            //DeleteOldEssence();
        }

        private void DeleteOldEssence()
        {
            if (env.counter % essence_update_interval * 10 == 0)
            {
                int c = ai.essences.Count;
                if (c == 0)
                {
                    return;
                }
                for (int i = 0; i < c; i++)
                {
                    if (ai.essences[i].shell == null)
                    {
                        ai.essences[i].age += env.timeInterval;
                        if (ai.essences[i].age > 300)
                        {
                            ai.RemoveEssence(ai.essences[i]);
                            return;
                        }
                    }
                }
            }
        }

        private void ApplyCurrent()
        {
            //update
            PointF add;
            add = f.NormDist_RandomPoint(new PointF(), current_direction_sd);

            current_direction = f.Add(current_direction, add);
            current_magnitude = current_magnitude + f.NormDist_RandomX(0, current_magnitude_sd);

            force_current = f.Offset(current_direction, current_magnitude);

            //apply
            int c = ai.beings.Count;
            for (int i = 0; i < c; i++)
            {
                ai.beings[i].force = f.Add(ai.beings[i].force, force_current);
            }
            c = ai.essences.Count;
            for (int i = 0; i < c; i++)
            {
                ai.essences[i].force = f.Add(ai.essences[i].force, force_current);
            }
            c = ai.shells.Count;
            for (int i = 0; i < c; i++)
            {
                ai.shells[i].force = f.Add(ai.shells[i].force, force_current);
            }
        }

        public void EatAll(Being being)
        {
            if (being.timer_feed != 0)
            {
                return;
            }

            //check to see if anything is inside beings mouth position
            PointF p;
            int c, cc, ccc;
            bool found;

            p = f.Add(being.p, being.mouth_offset);

            ////beings
            //c = ai.beings.Count;
            //if (c != 0)
            //{
            //    for (int i = 0; i < c; i++)
            //    {
            //        if (ai.beings[i] != being && f.Distance(ai.beings[i].p, p) <= being_feed_radius_coefficient * being.radius)
            //        {
            //            Eat(being, ai.beings[i]);
            //        }
            //    }
            //}
            //protein
            try
            {
                IEnumerator<Essence> en = ai.essences.GetEnumerator();
                Essence essence;

                while (en.MoveNext())
                {
                    essence = en.Current;

                    if (f.Distance(essence.p, being.p) <= being_feed_radius_coefficient * being.radius)
                    {
                        //chack that it is not one of own
                        cc = being.shells.Count;
                        found = false;
                        if (cc != 0)
                        {
                            for (int v = 0; v < cc; v++)
                            {
                                ccc = being.shells[v].essences.Count;
                                if (ccc != 0)
                                {
                                    for (int b = 0; b < ccc; b++)
                                    {
                                        if (essence == being.shells[v].essences[b])
                                        {
                                            found = true;
                                            break;
                                        }
                                    }
                                }
                                if (found)
                                {
                                    break;
                                }
                            }
                        }
                        if (!found)
                        {
                            Eat(being, essence);
                        }
                    }
                }
                en.Dispose();
            }
            catch { }
        }
        private void Eat(Being being, Essence essence)
        {
            env.ai.AddEatenEssence(essence.p);

            env.ai.AllocateEssenceToShell(being, essence);

            being.feeding = false;
            being.timer_feed = being_feed_timer_start_s;
            being.unsucessful_feeds_short = 0;
            being.unsucessful_feeds_long = 0;

            env.vis.PlaySound_Being_Eat();
        }

        void RandomEssenceMovement()
        {
            if (env.counter % essence_update_interval == 0)
            {
                int c = ai.essences.Count;
                for (int i = 0; i < c; i++)
                {
                    if (ai.essences[i].shell == null)
                    {
                        ai.essences[i].force = f.NormDist_RandomPoint(new PointF(), essence_movement_force_sd);
                        ai.essences[i].force = f.Multiply(ai.essences[i].force, ai.essences[i].radius);
                        ai.essences[i].force = f.Multiply(ai.essences[i].force, ai.essences[i].radius);

                        ai.essences[i].velocity = f.Multiply(ai.essences[i].velocity, frictionPerTick);
                    }
                    ai.essences[i].age += env.timeInterval;
                }
            }
        }

        void RealiseForce()
        {
            int c = ai.beings.Count;
            for (int i = 0; i < c; i++)
            {
                RealiseForce(ai.beings[i]);
            }

            if (env.counter % essence_update_interval == 0)
            {
                c = ai.essences.Count;
                for (int i = 0; i < c; i++)
                {
                    if (ai.essences[i].shell == null)
                    {
                        RealiseForce(ai.essences[i]);
                    }
                }
            }

            c = ai.shells.Count;
            for (int i = 0; i < c; i++)
            {
                RealiseForce(ai.shells[i]);
            }
        }
        public void RealiseForce(Being being)
        {
            PointF acc, vChange, dist;

            acc = f.Divide(being.force, being.mass);

            vChange = f.Multiply(acc, env.timeInterval);

            //realise velocity
            being.velocity = f.Add(being.velocity, vChange);

            //apply friction
            being.velocity = f.Multiply(being.velocity, frictionPerTick);

            //realise distance
            dist = f.Multiply(being.velocity, env.timeInterval);
            being.p = f.Add(being.p, dist);



            //update mouth position
            PointF dir;

            if (being.velocity != INF_PointF && being.velocity != new PointF())
            {
                dir = being.velocity;
            }
            else
            {
                dir = new PointF(0, -1);
            }

            being.mouth_offset = f.Offset(dir, being.radius * 1.2f);
        }
        public void RealiseForce(Essence essence)
        {
            PointF acc, vChange, dist;

            acc = f.Divide(essence.force, essence.mass);

            vChange = f.Multiply(acc, env.timeInterval * (float)essence_update_interval);

            //realise velocity
            essence.velocity = f.Add(essence.velocity, vChange);

            //apply friction
            for (int i = 0; i < essence_update_interval; i++)
            {
                essence.velocity = f.Multiply(essence.velocity, frictionPerTick);
            }

            //realise distance
            dist = f.Multiply(essence.velocity, env.timeInterval * (float)essence_update_interval);
            essence.p = f.Add(essence.p, dist);
        }
        public void RealiseForce(Shell shell)
        {
            PointF acc, vChange, dist;

            acc = f.Divide(shell.force, shell.mass);

            vChange = f.Multiply(acc, env.timeInterval);

            //realise velocity
            shell.velocity = f.Add(shell.velocity, vChange);

            //apply friction
            shell.velocity = f.Multiply(shell.velocity, frictionPerTick);

            //realise distance
            dist = f.Multiply(shell.velocity, env.timeInterval);
            shell.p = f.Add(shell.p, dist);
        }

        void CorrectDistance()
        {
            //correct protein distances of beings
            int c, cc, ccc;

            c = ai.beings.Count;
            for (int i = 0; i < c; i++)
            {
                cc = ai.beings[i].shells.Count;
                for (int v = 0; v < cc; v++)
                {
                    CorrectDistance(ai.beings[i].shells[v]);

                    ccc = ai.beings[i].shells[v].essences.Count;
                    if (ccc != 0)
                    {
                        for (int b = 0; b < ccc; b++)
                        {
                            CorrectDistance(ai.beings[i].shells[v].essences[b]);
                        }
                    }
                }
            }
        }
        public void CorrectDistance(Shell shell)
        {
            PointF dir, dist, newP, vChange;
            int index;
            float friction;

            float distance_multiplier = 1.7f;
            float friction_power_multiplier = 3f;
            float friction_index_sensitivity = 0.9f;

            index = shell.being.shells.IndexOf(shell);

            if (index == 0)
            {
                dir = f.Dir(shell.being.p, shell.p);
                dir = f.Offset(dir, (shell.radius + shell.being.radius) * distance_multiplier);

                newP = f.Add(shell.being.p, dir);
            }
            else
            {
                Shell prevShell = shell.being.shells[index - 1];

                dir = f.Dir(prevShell.p, shell.p);
                dir = f.Offset(dir, (shell.radius + prevShell.radius) * distance_multiplier);

                newP = f.Add(prevShell.p, dir);
            }

            dist = f.Diff(shell.p, newP);

            //realise distance
            shell.p = newP;

            friction = (float)Math.Pow(frictionPerTick, friction_power_multiplier * Math.Pow(index + 1, friction_index_sensitivity));
            friction = f.NumberBound(friction, 0.5f);

            //update velocity:
            //change in v = d/t
            vChange = f.Divide(dist, env.timeInterval);
            shell.velocity = f.Add(shell.velocity, vChange);
            shell.velocity = f.Multiply(shell.velocity, friction);
        }
        public void CorrectDistance(Essence essence)
        {
            if (essence.shell == null)
            {
                return;
            }

            essence.p = essence.shell.p;
        }

        void CheckBounds()
        {
            //check positions against bounds

            int c = ai.beings.Count;
            for (int i = 0; i < c; i++)
            {
                ai.beings[i].p = CheckBounds(ai.beings[i].p);
            }
        }
        public PointF CheckBounds(PointF p)
        {
            //PointF topBound, lowBound;

            //topBound = env.vis.e.ClipRectangle.Location;
            //lowBound = new PointF(topBound.X + env.vis.e.ClipRectangle.Width, topBound.Y + env.vis.e.ClipRectangle.Height);
            ////lowBound.Y -= 20;

            ////vertical
            ////ceiling
            //if (p.Y < topBound.Y)
            //{
            //    p.Y = topBound.Y;
            //}
            ////floor
            //else if (p.Y > lowBound.Y)
            //{
            //    p.Y = lowBound.Y;
            //}

            ////horizontal
            ////left
            //if (p.X < topBound.X)
            //{
            //    p.X = topBound.X;
            //}
            ////right
            //else if (p.X > lowBound.X)
            //{
            //    p.X = lowBound.X;
            //}

            return p;
        }
    }
}
