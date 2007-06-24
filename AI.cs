using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Collections.ObjectModel;
using System.Threading;

namespace AI
{
    class AI
    {
        Env env;
        F f;
        internal Collection<Being> beings;
        internal Collection<Essence> essences;
        internal Collection<Shell> shells;
        internal Collection<PointF> eatenEssence;
        PointF INF_PointF, center;
        Random rn;
        int counter, idCounter;
        public int restarts;
        public float age;

        //settings:
        public int essence_types = 2;
        float density_being = 1.0f;
        float density_essence = 0.5f;
        float density_shell = 0.2f;
        public float spawn_spread = 400f;
        int spawn_beings = 5;
        int spawn_essence = 30;
        float being_lookAround_interval = 0.1f;
        public float being_lookAround_radius = 100f;
        float being_propulsion_force_max = 130000f;
        float being_rotation_maximum_radians = 1.0f;
        float protein_mass_to_amount_ratio = 134f;
        float protein_amount_mean = 0.6f;
        float protein_amount_sd = 8f;
        public float timer_creation = 0.2f;
        float timer_divide_s = 15f;
        float being_dir_change_sd = 0.02f;
        int being_unsucessful_feeds_max = 3;
        float being_memory_long_unvisited_max = 15f;
        public float essence_age_max = 600f;

        public AI(Env _env)
        {
            env = _env;
            f = env.f;

            beings = new Collection<Being>();
            essences = new Collection<Essence>();
            shells = new Collection<Shell>();

            eatenEssence = new Collection<PointF>();

            INF_PointF = new PointF(float.PositiveInfinity, float.PositiveInfinity);
            rn = new Random((int)DateTime.Now.Ticks);

            center = new PointF(0, 0);

            CreateWorld();
        }

        public Being NewBeing()
        {
            return NewBeing(NormDist_RandomPoint_WithinBounds(center, spawn_spread), 0);
        }
        public Being NewBeing(PointF p, int generation)
        {
            float volume;
            Being being = new Being(NewId());
            Shell shell;

            being.amount = new float[essence_types];
            being.oldAmount = new float[essence_types, 3];

            being.p = p;
            being.radius = 6f;
            being.timer_creation = timer_creation;
            being.timer_divide = timer_divide_s;
            being.generation = generation;

            being.memory_essence.min_spread = spawn_spread;

            for (int i = 0; i < essence_types; i++)
            {
                being.amount[i] = 1.0f;
            }

            //shell = NewShell(being, being.radius * 0.8f);
            //NewShell(being, being.radius * 0.6f);
            //NewShell(being, being.radius * 0.3f);

            //shell.essences.Add(NewEssence(shell, new PointF(), 1f, 0));

            //4/3*PI*r*3
            volume = (float)(4.0 / 3.0 * Math.PI * Math.Pow(being.radius, 3));
            being.mass = volume * density_being;

            ThreadStart starter = delegate { ApplyBeing(being); };
            being.thread = new Thread(starter);
            being.thread.Start();

            beings.Add(being);

            return being;
        }
        public Shell NewShell(Being being, float radius)
        {
            float volume;
            Shell shell = new Shell(NewId());

            shell.being = being;
            shell.radius = radius;
            shell.p = new PointF(being.p.X, being.p.Y - being.radius - shell.radius);

            //4/3*PI*r*3
            volume = (float)(4.0 / 3.0 * Math.PI * Math.Pow(shell.radius, 3));
            shell.mass = volume * density_shell;

            being.shells.Add(shell);
            shells.Add(shell);

            //ThreadStart starter = delegate { ApplyShell(shell); };
            //being.thread = new Thread(starter);
            //being.thread.Start();

            return shell;
        }
        public Essence NewEssence()
        {
            return NewEssence(rn.Next(0, essence_types));
        }
        public Essence NewEssence(PointF p)
        {
            float amount = f.Abs(f.NormDist_RandomX(protein_amount_mean, protein_amount_sd));
            amount = f.NumberBound(amount, 0.1f);

            return NewEssence(null, p, amount, rn.Next(0, essence_types));
        }
        public Essence NewEssence(int type)
        {
            PointF p = NormDist_RandomPoint_WithinBounds(center, spawn_spread);
            float amount = f.Abs(f.NormDist_RandomX(protein_amount_mean, protein_amount_sd));
            amount = f.NumberBound(amount, 0.1f);

            return NewEssence(null, p, amount, type);
        }
        public Essence NewEssence(Shell shell, PointF p, float amount, int type)
        {

            Essence essence = new Essence(NewId());

            essence.shell = shell;
            essence.timer_creation = timer_creation;

            NewEssenceAmount(essence, amount, type);

            if (shell == null)
            {
                essence.p = p;
            }
            else
            {
                essence.p = new PointF(shell.p.X, shell.p.Y + shell.radius + essence.radius);
            }

            ThreadStart starter = delegate { ApplyEssence(essence); };
            essence.thread = new Thread(starter);
            essence.thread.Start();

            essences.Add(essence);

            return essence;
        }

        public void Apply()
        {
            //if (beings.Count == 0)
            //{
            //    return;
            //}

            //ResetForces();

            //Think();
            //Act();

            //BodyFunction();
            //Remove();
            //BeingDivide();

            //-----
            IncrementCounter();
            age += env.timeInterval;
        }

        public void ApplyBeing(Being being)
        {
            while (being.thread.IsAlive)
            {
                if (env.phy != null)
                {
                    bool retry = true;
                    while (retry)
                    {
                        retry = false;
                        try
                        {
                            being.force = new PointF();

                            //AI
                            LookAround(being);
                            Think(being);
                            Act(being);

                            BodyFunction(being);

                            for (int v = 0; v < essence_types; v++)
                            {
                                if (being.amount[v] <= 0)
                                {
                                    RemoveBeing(being);
                                    return;
                                }
                            }

                            if (being.timer_divide <= 0)
                            {
                                BeingDivide(being);
                            }

                            //Physics
                            env.phy.RealiseForce(being);

                            int c = being.shells.Count;
                            for (int i = 0; i < c; i++)
                            {
                                ApplyShell(being.shells[i]);

                                int cc = being.shells[i].essences.Count;
                                for (int v = 0; v < cc; v++)
                                {
                                    if (being.shells[i].essences.Count != cc)
                                    {
                                        break;
                                    }
                                    ApplyEssence(being.shells[i].essences[v]);
                                }
                            }

                            being.updated = true;
                        }
                        catch
                        {
                            retry = true;
                        }
                    }
                }

                Thread.Sleep(env.timer.Interval + rn.Next(-3, 3));
            }
        }
        public void ApplyShell(Shell shell)
        {
            if (!env.alive)
            {
                Thread.Sleep(env.timer.Interval);
                return;
            }

            if (shell.being == null)
            {
                return;
            }

            if (env.phy != null)
            {
                //Physics
                env.phy.RealiseForce(shell);
                env.phy.CorrectDistance(shell);
            }
        }
        public void ApplyEssence(Essence essence)
        {
            if (!env.alive)
            {
                Thread.Sleep(env.timer.Interval);
                return;
            }

            bool loop = true;

            while (loop)
            {
                if (essence.shell != null && Thread.CurrentThread == essence.thread)
                {
                    Thread.Sleep(env.timer.Interval * 2);
                }
                else
                {
                    if (env.phy != null)
                    {
                        bool retry = true;
                        while (retry)
                        {
                            retry = false;
                            try
                            {
                                //Physics
                                if (essence.shell == null)
                                {
                                    essence.force = f.NormDist_RandomPoint(new PointF(), env.phy.essence_movement_force_sd);
                                    essence.force = f.Multiply(essence.force, essence.radius);
                                    essence.force = f.Multiply(essence.force, essence.radius);

                                    essence.velocity = f.Multiply(essence.velocity, env.phy.frictionPerTick);

                                    essence.age += env.timeInterval;

                                    env.phy.RealiseForce(essence);

                                    loop = true;
                                }
                                else
                                {
                                    loop = false;
                                    essence.age = 0;
                                }
                                env.phy.CorrectDistance(essence);

                                if (essence.amount <= 0 || essence.age > 300)
                                {
                                    RemoveEssence(essence);
                                    return;
                                }
                            }
                            catch
                            {
                                retry = true;
                            }
                        }
                    }

                    if (essence.shell == null)
                    {
                        Thread.Sleep(env.timer.Interval * 2 + rn.Next(-3, 3));
                    }
                }

                if (!essence.thread.IsAlive)
                {
                    loop = false;
                }
            }
        }

        void ResetForces()
        {
            int c = beings.Count;
            for (int i = 0; i < c; i++)
            {
                beings[i].force = new PointF();
            }

            c = essences.Count;
            for (int i = 0; i < c; i++)
            {
                essences[i].force = new PointF();
            }

            c = shells.Count;
            for (int i = 0; i < c; i++)
            {
                shells[i].force = new PointF();
            }
        }

        void NewEssenceAmount(Essence essence, float amount, int type)
        {
            float volume;

            essence.amount = amount;
            essence.type = type;

            essence.mass = amount * protein_mass_to_amount_ratio;
            volume = essence.mass / density_essence;

            //V = 4/3*PI*r*3
            //r = Pow(3*V/4/PI,1/3)
            essence.radius = (float)Math.Pow(3f * volume / 4f / Math.PI, 1f / 3f);
            essence.radius = f.NumberBound(essence.radius, 1);

            //position
            if (type == 2)
            {
                essence.external = true;
            }
        }

        void BodyFunction()
        {
            int c = beings.Count;
            for (int i = 0; i < c; i++)
            {
                BodyFunction(beings[i]);
            }
        }
        void BodyFunction(Being being)
        {
            float essence_per_unitForce = 0.00000005f;
            float essence_transfer_per_s = 0.08f;
            float essence_per_s = 0.01f;

            being.counter++;

            being.timer_memory_long_unvisited += env.timeInterval;
            if (being.memory_essence.count_long == 0 || f.Distance(being.p, being.memory_essence.mean_long) <= being.memory_essence.sd_long * 3f)
            {
                being.timer_memory_long_unvisited = 0;
            }

            //determine amount of essence avaliable for movement
            float sensitivity = 2f;

            float leastAmount, max, coefficient;

            f.MinMaxValue(being.amount, out leastAmount, out max);
            being.aveOldAmount = f.Mean_Row(being.oldAmount);

            being.health = leastAmount;
            being.minEssenceType = Array.IndexOf(being.amount, leastAmount);

            coefficient = f.NumberBound(leastAmount, 0f, 1f);
            coefficient = (float)Math.Pow(coefficient, sensitivity);
            being.propulsion_force = coefficient * being_propulsion_force_max;

            being.oldAmount = f.UpdateRows(being.oldAmount, being.amount);
            being.timer_life += env.timeInterval;

            float essence_transfer_per_tick, essence_per_tick, amount;

            essence_transfer_per_tick = essence_transfer_per_s * env.timeInterval;
            essence_per_tick = essence_per_s * env.timeInterval;

            //use up locomotive energy
            for (int i = 0; i < essence_types; i++)
            {
                being.amount[i] -= essence_per_unitForce * being.propulsion_force;
                being.amount[i] -= essence_per_tick;
            }

            //transfer essence
            int c, cc, type;
            c = being.shells.Count;
            if (c != 0)
            {
                for (int i = 0; i < c; i++)
                {
                    cc = being.shells[i].essences.Count;
                    for (int v = 0; v < cc; v++)
                    {
                        type = being.shells[i].essences[v].type;
                        if (being.amount[type] < 1f)
                        {
                            amount = 1f - being.amount[type];
                            amount = f.NumberBound(amount, 0, essence_transfer_per_tick);

                            //transfer
                            if (being.shells[i].essences[v].amount - amount <= 0)
                            {
                                //finished
                                being.amount[type] += being.shells[i].essences[v].amount;
                                being.shells[i].essences[v].amount = 0;
                            }
                            else
                            {
                                being.shells[i].essences[v].amount -= amount;
                                being.amount[type] += amount;
                            }

                            NewEssenceAmount(being.shells[i].essences[v], being.shells[i].essences[v].amount, being.shells[i].essences[v].type);
                        }
                    }
                }
            }

            //feed timer
            if (being.timer_feed > 0)
            {
                being.timer_feed -= env.timeInterval;
            }
            else if (being.timer_feed < 0)
            {
                being.timer_feed = 0;
            }

            //divide
            bool healthy = true;

            if (being.shells.Count >= 8)
            {
                for (int i = 0; i < essence_types; i++)
                {
                    if (being.amount[i] < 1f)
                    {
                        healthy = false;
                    }
                }
            }
            else
            {
                healthy = false;
            }

            if (healthy)
            {
                being.timer_divide -= env.timeInterval;
                if (being.timer_divide < 0)
                {
                    being.timer_divide = 0;
                }
            }
            else
            {
                being.timer_divide = timer_divide_s;
            }

            ////add parent to visibles if new
            //if (being.notFoundEssence && being.visibleBeings == 0)
            //{
            //    //being.visibleBeings.add
            //}
        }

        void Remove()
        {
            bool changed;

            //beings
            changed = true;
            while (changed)
            {
                changed = false;

                int c = beings.Count;
                for (int i = 0; i < c; i++)
                {
                    for (int v = 0; v < essence_types; v++)
                    {
                        if (beings[i].amount[v] <= 0)
                        {
                            RemoveBeing(beings[i]);
                            changed = true;
                            break;
                        }
                    }
                    if (changed)
                    {
                        break;
                    }
                }
            }

            //essence
            changed = true;
            while (changed)
            {
                changed = false;

                int c = essences.Count;
                for (int i = 0; i < c; i++)
                {
                    if (essences[i].amount <= 0)
                    {
                        RemoveEssence(essences[i]);
                        changed = true;
                        break;
                    }
                }
                if (changed)
                {
                    break;
                }

            }
        }
        void RemoveShell(Shell shell, bool delete)
        {
            if (!delete)
            {
                try
                {
                    IEnumerator<Essence> en = shell.essences.GetEnumerator();
                    Essence essence;

                    while (en.MoveNext())
                    {
                        essence = en.Current;

                        essence.shell = null;
                        essence.timer_creation = timer_creation;

                        //ThreadStart starter = delegate { ApplyEssence(shell.essences[i]); };
                        //shell.essences[i].thread = new Thread(starter);
                        //shell.essences[i].thread.Start();
                    }
                    en.Dispose();
                }
                catch { }
            }

            shell.being.shells.Remove(shell);
            shells.Remove(shell);

            while (shell.essences.Count != 0)
            {
                if (delete)
                {
                    RemoveEssence(shell.essences[0]);
                }
                else
                {
                    shell.essences.Remove(shell.essences[0]);
                }
            }
        }
        public void RemoveEssence(Essence essence)
        {
            essences.Remove(essence);
            if (essence.shell != null)
            {
                essence.shell.essences.Remove(essence);
            }

            if (env.vis.selected_hover_essence == essence)
            {
                env.vis.selected_hover_essence = null;
            }

            //remove from visibility

            essence.thread.Abort();
            essence.thread.Join();
        }
        void RemoveBeing(Being being)
        {
            beings.Remove(being);
            if (env.vis.selected_being == being)
            {
                env.vis.selected_being = null;
            }
            if (env.vis.selected_hover_being == being)
            {
                env.vis.selected_hover_being = null;
            }

            ////break shells
            //int c;
            //c = being.shells.Count;
            //if (c != 0)
            //{
            while (being.shells.Count != 0)
            {
                RemoveShell(being.shells[0], false);
            }
            //for (int i = 0; i < c; i++)
            //{
            //    //cc = being.shells[i].essences.Count;
            //    //if (cc != 0)
            //    //{
            //    //    for (int v = 0; v < cc; v++)
            //    //    {
            //    //        being.shells[i].essences[v].shell = null;
            //    //        being.shells[i].essences[v].timer_creation = timer_creation;
            //    //    }
            //    //}
            //    //shells.Remove(being.shells[i]);
            //}
            //}

            //remove from visibility
            int c = beings.Count;
            if (c != 0)
            {
                for (int i = 0; i < c; i++)
                {
                    beings[i].visibleBeings.Remove(being);
                }
            }

            env.vis.PlaySound_Being_Die();

            being.thread.Abort();
            being.thread.Join();
        }

        void BeingDivide()
        {
            bool changed;

            //beings
            changed = true;
            while (changed)
            {
                changed = false;

                int c = beings.Count;
                for (int i = 0; i < c; i++)
                {
                    if (beings[i].timer_divide <= 0)
                    {
                        BeingDivide(beings[i]);
                        changed = true;
                        break;
                    }
                }
            }
        }
        void BeingDivide(Being being)
        {
            //half of shells go to new being
            //middle shell turns into head of new being
            int middle, c;
            Being being2;
            Shell middleShell, shell;

            middle = (int)Math.Floor((double)being.shells.Count / 2.0);
            middleShell = being.shells[middle];
            //change shell into head
            being2 = NewBeing(middleShell.p, being.generation + 1);
            RemoveShell(middleShell, true);

            //transfer shells
            int i = middle;
            while (being.shells.Count > middle)
            {
                shell = being.shells[i];
                shell.being = being2;
                being2.shells.Add(shell);
                being.shells.Remove(shell);
            }

            being.timer_divide = timer_divide_s;
            being.divisions++;

            env.vis.PlaySound_Being_Born();
        }

        void Think()
        {
            int c = beings.Count;
            for (int i = 0; i < c; i++)
            {
                LookAround(beings[i]);
                Think(beings[i]);
            }
        }
        void Think(Being being)
        {
            //movement:
            //if new: follow visible beings: status 0
            if (being.notFoundEssence && being.visibleBeings.Count != 0 && !being.visibleBeings[0].notFoundEssence)
            {
                //go beside closest visible being
                PointF p, dir, diff;
                Being cb;

                cb = GetClosest(being.visibleBeings, being.p);
                p = cb.p;
                dir = cb.velocity;
                if (dir == new PointF())
                {
                    dir = new PointF(0, -1);
                }
                dir = f.Opposite(dir);

                diff = f.Dir(cb.p, being.p);

                dir = f.Add(dir, f.Multiply(f.Perpendicular(dir, f.LeftOfVector(dir, diff)), 2f));
                dir = f.Offset(dir, (cb.radius + being.radius) * 3f);

                being.target_move = f.Add(p, dir);

                being.status = 0;
            }
            else
            {
                //if havent tried long term memory in a while
                if (being.timer_memory_long_unvisited >= being_memory_long_unvisited_max)
                {
                    being.unsucessful_feeds_long = 0;
                }

                //long term
                if (being.memory_essence.count_long > 1 && being.unsucessful_feeds_long < (int)((float)being_unsucessful_feeds_max * 1.8f) & being.memory_essence.count_short == 0)
                {
                    if (BeingAtPosition(being, being.target_move) | being.target_move == INF_PointF)
                    {
                        //move about the memory mean: status 1
                        being.target_move = f.NormDist_RandomPoint(being.memory_essence.mean_long, being.memory_essence.sd_long);
                        //if point is inside long term radius then add to it
                        if (f.Distance(being.p, being.memory_essence.mean_long) <= being.memory_essence.sd_long * 3f)
                        {
                            being.unsucessful_feeds_long++;
                            //if (being.unsucessful_feeds_long >= (int)((float)being_unsucessful_feeds_max * 1.8f))
                            //{
                            //    being.memory_essence.Clear_Long();
                            //}
                        }
                        being.status = 1;
                    }
                }
                //move in a random direction: status 2
                else if (being.memory_essence.count_short == 0)
                {
                    //change change in direction randomly
                    being.changeDir = f.NormDist_RandomPoint(being.changeDir, being_dir_change_sd);
                    being.changeDir = f.Offset(being.changeDir, 0.3f);
                    being.dir = f.Add(being.dir, being.changeDir);
                    being.dir = f.Offset(being.dir, 2f);
                    being.target_move = INF_PointF;

                    being.status = 2;
                }
                //short term
                else if (BeingAtPosition(being, being.target_move) | being.target_move == INF_PointF)
                {
                    //move about the memory mean: status 3
                    being.target_move = f.NormDist_RandomPoint(being.memory_essence.mean_short, being.memory_essence.sd_short);
                    if (being.timer_feed == 0)
                    {
                        //if point is inside short term radius then add to it
                        if (f.Distance(being.p, being.memory_essence.mean_short) <= being.memory_essence.sd_short * 3f)
                        {
                            being.unsucessful_feeds_short++;
                        }
                        //if point is inside long term radius then add to it
                        if (f.Distance(being.p, being.memory_essence.mean_long) <= being.memory_essence.sd_long * 3f)
                        {
                            being.unsucessful_feeds_long++;
                        }
                    }

                    being.status = 3;
                }

                //move towards food if visible: status 4
                if (being.visibleEssence.Count != 0 && being.timer_feed == 0)
                {
                    being.target_move = GetClosest(being.visibleEssence, being.p).p;

                    being.status = 4;
                }

                //eat food if adjacent
                if (BeingAtPosition(being, being.target_move))
                {
                    env.phy.EatAll(being);
                }

                //if unsucessful feeds is significant, erase short term memory, i.e. start again
                if (being.unsucessful_feeds_short >= being_unsucessful_feeds_max)
                {
                    being.target_move = INF_PointF;
                    being.memory_essence.DumpShortToLong();

                    //set direction towards long term memory mean
                    if (being.memory_essence.count_long != 0)
                    {
                        //PointF p;
                        //p = f.NormDist_RandomPoint(being.memory_essence.mean_long, being.memory_essence.sd_long);
                        being.dir = f.Dir(being.p, being.memory_essence.mean_long);
                    }
                    else
                    {
                        //pick random direction
                        being.dir = f.Add(new PointF(), being.changeDir);
                        being.dir = f.Offset(being.dir, 2f);
                    }
                    being.unsucessful_feeds_short = 0;
                }
            }
        }

        void LookAround(Being being)
        {
            if ((int)(being_lookAround_interval / env.timeInterval) == 0 || being.counter % (int)(being_lookAround_interval / env.timeInterval) == 0)
            {
                float dist;
                int c, cc, ccc;
                bool found, changed;

                //beings
                being.visibleBeings.Clear();
                IEnumerator<Being> en = env.ai.beings.GetEnumerator();
                Being being2;

                while (en.MoveNext())
                {
                    being2 = en.Current;

                    if (being2 != being && f.Distance(being2.p, being.p) <= being_lookAround_radius)
                    {
                        being.visibleBeings.Add(being2);
                    }
                }
                en.Dispose();

                //essence
                being.visibleEssence.Clear();
                IEnumerator<Essence> en2 = env.ai.essences.GetEnumerator();
                Essence essence;

                while (en2.MoveNext())
                {
                    essence = en2.Current;

                    if (f.Distance(essence.p, being.p) <= being_lookAround_radius)
                    {
                        //check that its not eaten
                        if (!EssenceTaken(essence))
                        {
                            being.visibleEssence.Add(essence);
                            being.memory_essence.AddMemory(essence.p, essence.id);
                            being.notFoundEssence = false;
                        }
                        else
                        {
                            if (essence.shell.being != being)
                            {
                                being.visibleBeings = AddToCol(being.visibleBeings, essence.shell.being);
                            }
                        }

                        ////check that it is not one of own
                        //cc = being.shells.Count;
                        //found = false;
                        //if (cc != 0)
                        //{
                        //    for (int v = 0; v < cc; v++)
                        //    {
                        //        ccc = being.shells[v].essences.Count;
                        //        if (ccc != 0)
                        //        {
                        //            for (int b = 0; b < ccc; b++)
                        //            {
                        //                if (essences[i] == being.shells[v].essences[b])
                        //                {
                        //                    found = true;
                        //                    break;
                        //                }
                        //            }
                        //        }
                        //        if (found)
                        //        {
                        //            break;
                        //        }
                        //    }
                        //}
                        //if (!found)
                        //{
                        //    being.visibleEssence.Add(essences[i]);
                        //}
                    }
                }
                en2.Dispose();
            }
        }

        public Collection<Being> AddToCol(Collection<Being> col, Being being)
        {
            int c = col.Count;
            if (c == 0)
            {
                col.Add(being);
                return col;
            }

            //check for repeat
            for (int i = c - 1; i >= 0; i--)
            {
                if (col[i] == being)
                {
                    return col;
                }
            }

            col.Add(being);
            return col;
        }

        public bool EssenceTaken(Essence essence)
        {
            if (essence.shell != null)
            {
                return true;
            }
            return false;

            //int c = beings.Count;
            //for (int i = 0; i < c; i++)
            //{
            //    if (EssenceBelongsTo(essence, beings[i]))
            //    {
            //        return true;
            //    }
            //}

            //return false;
        }
        bool EssenceBelongsTo(Essence essence, Being being)
        {
            if (essence.shell != null && essence.shell.being == being)
            {
                return true;
            }
            return false;

            //int cc, ccc;
            //bool found;

            ////chack that it is not one of own
            //cc = being.shells.Count;
            //found = false;
            //if (cc != 0)
            //{
            //    for (int v = 0; v < cc; v++)
            //    {
            //        ccc = being.shells[v].essences.Count;
            //        if (ccc != 0)
            //        {
            //            for (int b = 0; b < ccc; b++)
            //            {
            //                if (essence == being.shells[v].essences[b])
            //                {
            //                    found = true;
            //                    break;
            //                }
            //            }
            //        }
            //        if (found)
            //        {
            //            break;
            //        }
            //    }
            //}
            //return found;
        }

        public void AllocateEssenceToShell(Being being, Essence essence)
        {
            if (essence.shell != null)
            {
                essence.shell.essences.Remove(essence);
            }
            essence.age = 0;

            //find closest (to head) shell that has a bigger radius
            //and does not have any essence with similar radius
            //inside it

            int c, cc;
            Shell shell;
            bool ok, foundAny;

            float gap_threshhold = 2.2f;

            foundAny = false;

            c = being.shells.Count;
            for (int i = 0; i < c; i++)
            {
                shell = being.shells[i];

                //check shell
                //if (shell.radius > essence.radius)
                //{
                //check shells essences
                ok = true;
                cc = shell.essences.Count;
                for (int v = 0; v < cc; v++)
                {
                    if (shell.essences[v].radius + gap_threshhold > essence.radius && shell.essences[v].radius - gap_threshhold < essence.radius)
                    {
                        ok = false;
                        break;
                    }
                }
                if (ok)
                {
                    shell.essences.Add(essence);
                    essence.shell = shell;

                    foundAny = true;

                    //temp:
                    if (shell.radius < essence.radius)
                    {
                        shell.radius = essence.radius;
                    }
                    break;
                }
                //}
            }
            if (!foundAny)
            {
                //create new shell
                shell = NewShell(being, essence.radius);
                shell.essences.Add(essence);
                essence.shell = shell;
            }

            essence.timer_creation = timer_creation;
        }

        Essence GetClosest(Collection<Essence> col, PointF p)
        {
            int c = col.Count;
            if (c == 0)
            {
                return null;
            }

            //check for closest
            float dist, closestDist, minDistMultiplier;
            Essence closest;

            closest = null;
            closestDist = float.MaxValue;

            for (int i = 0; i < c; i++)
            {
                dist = f.Distance(p, col[i].p);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closest = col[i];
                }
            }

            return closest;
        }
        Being GetClosest(Collection<Being> col, PointF p)
        {
            int c = col.Count;
            if (c == 0)
            {
                return null;
            }

            //check for closest
            float dist, closestDist, minDistMultiplier;
            Being closest;

            closest = null;
            closestDist = float.MaxValue;

            for (int i = 0; i < c; i++)
            {
                dist = f.Distance(p, col[i].p);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closest = col[i];
                }
            }

            return closest;
        }

        void Act()
        {
            int c = beings.Count;
            for (int i = 0; i < c; i++)
            {
                Act(beings[i]);
            }
        }
        void Act(Being being)
        {
            if (being.target_move != INF_PointF)
            {
                MoveTo(being, being.target_move);
            }
            else if (being.memory_essence.count_short < 1)
            {
                Move(being);
            }
        }

        /// <summary>
        /// The being moves to destination. 
        /// </summary>
        void MoveTo(Being being, PointF p)
        {
            if (BeingAtPosition(being, p))
            {
                return;
            }

            PointF dir, oldDir, force, vChange, pForce, vForce;
            float pForceSize, vForceSize, angle, time, coefficient;

            //the direction can change only by a certain amount
            //(to avoid jumpy 180 changes)

            oldDir = being.velocity;
            dir = f.Dir(being.p, p);

            angle = f.AngleBetween(dir, oldDir);

            if (angle > being_rotation_maximum_radians)
            {
                dir = f.CircularMovement_Angle(new PointF(), oldDir, being_rotation_maximum_radians, f.RightOfVector(oldDir, dir));
            }

            //the force should not only be used to propel the being towards
            //the target but to change the velocity so that it heads
            //towards the target.

            //find change in velocity
            vChange = f.Dir(being.velocity, f.Offset(dir, f.Magnitude(being.velocity)));

            //find the force required to change to that velocity
            //F = m * change in v / t
            vForce = f.Multiply(f.Divide(vChange, env.timeInterval), being.mass);

            //whats left of the force goes to direct propulsion towards target
            vForceSize = f.NumberBound(f.Magnitude(vForce), 0, being.propulsion_force);
            pForceSize = being.propulsion_force - vForceSize;

            ////if reaching destination soon, reduce propulsion force
            //float seconds_min = 2f;

            //time = f.Magnitude(f.Dir(being.p, p)) / f.Magnitude(being.velocity);
            //time = f.NumberBound(time, 0, seconds_min);
            //coefficient = time / seconds_min;

            //being.propulsion_force -= pForceSize;
            //pForceSize = pForceSize * coefficient;
            //being.propulsion_force += pForceSize;

            //add propulsion force and velocity force
            vForce = f.Offset(vForce, vForceSize);
            pForce = f.Offset(dir, pForceSize);
            force = f.Add(vForce, pForce);

            being.force = f.Add(being.force, force);
            being.dir = dir;
        }
        void Move(Being being)
        {
            PointF force;

            force = f.Offset(being.dir, being.propulsion_force);

            being.force = f.Add(being.force, force);
        }

        //void Eat(Being being, Protein protein)
        //{

        //}
        //void Eat(Being being, Carbo carbo)
        //{

        //}

        bool BeingAtPosition(Being being, PointF p)
        {
            return (f.Distance(being.p, p) < being.radius);
        }

        PointF NormDist_RandomPoint_WithinBounds(PointF mean, double sd)
        {
            PointF p = f.NormDist_RandomPoint(mean, sd);

            //if (env.phy != null)
            //{
            //    p = env.phy.CheckBounds(p);
            //}

            return p;
        }

        public string Stats_Beings()
        {
            int c = beings.Count;
            if (c == 0)
            {
                return "";
            }

            string str;
            float totalHealth, totalGen, totalAge, totalDivisions;
            float aveHealth, aveGen, aveDivisions;
            TimeSpan aveAge;
            Being being;

            totalHealth = 0;
            totalGen = 0;
            totalAge = 0;
            totalDivisions = 0;

            for (int i = 0; i < c; i++)
            {
                being = beings[i];

                totalHealth += being.health;
                totalGen += being.generation;
                totalAge += being.timer_life;
                totalDivisions += being.divisions;
            }

            aveHealth = totalHealth / c;
            aveGen = totalGen / c;
            aveAge = new TimeSpan(f.SecondsToTicks(totalAge / c));
            aveDivisions = totalDivisions / c;

            str = Environment.NewLine +
                "n: " + c + Environment.NewLine +
                "health: " + string.Format("{0:0}", aveHealth * 100) + "%" + Environment.NewLine +
                "gen: " + string.Format("{0:0.0}", aveGen) + Environment.NewLine +
                "age: " + f.ToString(aveAge) + Environment.NewLine +
                "divisions: " + string.Format("{0:0.0}", aveDivisions) + Environment.NewLine;

            return str;
        }
        public string Stats_Essence()
        {
            int c = essences.Count;
            if (c == 0)
            {
                return "";
            }

            string str;
            float[] totalAmount, count;
            float aveAmount;

            totalAmount = new float[essence_types];
            count = new float[essence_types];

            for (int i = 0; i < c; i++)
            {
                totalAmount[essences[i].type] += essences[i].amount;
                count[essences[i].type]++;
            }

            str = Environment.NewLine;

            for (int i = 0; i < essence_types; i++)
            {
                aveAmount = totalAmount[i] / c;

                str += "type " + i + ": " + string.Format("{0:0.0}", aveAmount) + " (" + count[i] + ")(total: " + string.Format("{0:0.0}", totalAmount[i]) + ")" + Environment.NewLine;
            }

            return str;
        }

        public void AddEatenEssence(PointF p)
        {
            // keep at threshhold, update as needed

            int max = 10;

            int c = eatenEssence.Count;

            while (c >= max)
                eatenEssence.Remove(eatenEssence[eatenEssence.Count - 1]);
            
            eatenEssence.Add(p);
        }
        public void RemoveEatenEssence(PointF p)
        {
            int c = eatenEssence.Count;
            if (c == 0)
            {
                return;
            }

            eatenEssence.Remove(p);
        }

        void CreateWorld()
        {
            Being being;

            for (int i = 0; i < spawn_beings; i++)
            {
                being = NewBeing();
                for (int type = 0; type < essence_types; type++)
                {
                    AllocateEssenceToShell(being, NewEssence(type));
                }
            }
            for (int i = 0; i < spawn_essence; i++)
            {
                NewEssence();
            }

            age = 0;
        }

        public void Restart()
        {
            //clean everything
            beings.Clear();
            essences.Clear();
            shells.Clear();

            CreateWorld();

            restarts++;
            idCounter = 0;
            env.vis.PlaySound_Restart();

            env.vis.visPos = new PointF(env.Width / 2, env.Height / 2);
        }

        public void Dispose()
        {
            bool retry = true;

            while (retry)
            {
                retry = false;
                try
                {
                    IEnumerator<Being> en = beings.GetEnumerator();
                    Being being;

                    while (en.MoveNext())
                    {
                        being = en.Current;
                        being.thread.Abort();
                        being.thread.Join();
                    }
                }
                catch { retry = true; }
            }

            retry = true;
            while (retry)
            {
                retry = false;
                try
                {
                    IEnumerator<Essence> en = essences.GetEnumerator();
                    Essence essence;

                    while (en.MoveNext())
                    {
                        essence = en.Current;
                        essence.thread.Abort();
                        essence.thread.Join();
                    }
                }
                catch { retry = true; }
            }
        }

        void IncrementCounter()
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

        int NewId()
        {
            if (idCounter == int.MaxValue)
            {
                return 0;
                idCounter = 1;
            }
            else
            {
                idCounter++;
                return idCounter - 1;
            }
        }
    }

    class Being
    {
        public PointF p, target_move, velocity, force, mouth_offset, dir, changeDir;
        public float propulsion_force, mass, radius, health;
        public float[] amount, aveOldAmount;
        public float[,] oldAmount;
        public MemoryEssence memory_essence;
        internal Collection<Shell> shells;
        internal Collection<Being> visibleBeings;
        internal Collection<Essence> visibleEssence;
        public bool feeding, notFoundEssence;
        public int generation, divisions, id, unsucessful_feeds_short, unsucessful_feeds_long, status, minEssenceType;
        public float timer_feed, timer_creation, timer_divide;
        public float timer_life, timer_memory_long_unvisited;
        public int counter;
        public bool updated;

        public Thread thread;

        public Being(int _id)
        {
            target_move = new PointF(float.PositiveInfinity, float.PositiveInfinity);
            visibleBeings = new Collection<Being>();
            visibleEssence = new Collection<Essence>();
            shells = new Collection<Shell>();
            memory_essence = new MemoryEssence();
            id = _id;
            status = -1;
            updated = false;

            feeding = false;
            notFoundEssence = true;
        }

        private string DecodeStatus(int status)
        {
            switch (status)
            {
                case 0:
                    return "following another being." + Environment.NewLine;
                case 1:
                    return "looking for essence" + Environment.NewLine + "using long-term memory.";
                case 2:
                    return "looking for essence" + Environment.NewLine + "in a random direction.";
                case 3:
                    return "looking for essence" + Environment.NewLine + "using short-term memory.";
                case 4:
                    return "going to essence." + Environment.NewLine;
                default:
                    return "idle.";
            }
        }

        public override string ToString()
        {
            string str;
            float[] n;
            int c, cc;
            float min, max, ave, shellAve, total, shellTotal;
            int minType, maxType;
            TimeSpan time;

            time = new TimeSpan((long)(timer_life * 10000000.0f));

            str = Environment.NewLine + "health: " + string.Format("{0:0}", health * 100f) + "%" + Environment.NewLine +
                Environment.NewLine;

            str += "status: " + Environment.NewLine +
                DecodeStatus(status) + Environment.NewLine;

            if (health != 1f)
            {
                str += "lacking essence type " + minEssenceType + "." + Environment.NewLine +
                     Environment.NewLine;
            }
            else
            {
                str += Environment.NewLine + Environment.NewLine;
            }


            str += "gen: " + generation + Environment.NewLine +
                "age: " + time.Days + ":" + string.Format("{0:00}", time.Hours) + ":" + string.Format("{0:00}", time.Minutes) + ":" + string.Format("{0:00}", time.Seconds) + Environment.NewLine +
                "divisions: " + divisions + Environment.NewLine +
                "divide timer: " + string.Format("{0:0.0}", timer_divide) + "s" + Environment.NewLine +
                Environment.NewLine;

            str += "p: (" + string.Format("{0:0}", p.X) + "," + string.Format("{0:0}", p.Y) + ")" + Environment.NewLine +
                "v: (" + string.Format("{0:0}", velocity.X) + "," + string.Format("{0:0}", velocity.Y) + ")" + Environment.NewLine +
                "F: (" + string.Format("{0:0}", force.X) + "," + string.Format("{0:0}", force.Y) + ")" + Environment.NewLine +
                "propulsion F: " + string.Format("{0:0}", propulsion_force) + " N" + Environment.NewLine +
                "move target: (" + string.Format("{0:0}", target_move.X) + "," + string.Format("{0:0}", target_move.Y) + ")" + Environment.NewLine +
                Environment.NewLine;

            n = new float[amount.Length];
            shellTotal = 0;

            c = shells.Count;
            if (c != 0)
            {
                for (int i = 0; i < c; i++)
                {
                    cc = shells[i].essences.Count;
                    if (cc != 0)
                    {
                        for (int v = 0; v < cc; v++)
                        {
                            n[shells[i].essences[v].type] += shells[i].essences[v].amount;
                            shellTotal += shells[i].essences[v].amount;
                        }
                    }
                }
            }

            min = float.MaxValue;
            max = 0;
            minType = 0;
            maxType = 0;
            total = 0;

            c = amount.Length;
            for (int i = 0; i < c; i++)
            {
                if (amount[i] < min)
                {
                    min = amount[i];
                    minType = i;
                }
                if (amount[i] >= max)
                {
                    max = amount[i];
                    maxType = i;
                }

                total += amount[i];
            }

            float minChange, maxChange;
            string minChangeAt, maxChangeAt;

            minChange = amount[minType] - aveOldAmount[minType];
            maxChange = amount[maxType] - aveOldAmount[maxType];

            if (minChange == 0)
            {
                minChangeAt = "no change";
            }
            else
            {
                minChangeAt = "at " + string.Format("{0:0.000000}", minChange) + " per tick";
            }
            if (maxChange == 0)
            {
                maxChangeAt = "no change";
            }
            else
            {
                maxChangeAt = "at " + string.Format("{0:0.000000}", maxChange) + " per tick";
            }

            ave = total / n.Length;
            shellAve = shellTotal / n.Length;

            str += "essence: " + Environment.NewLine;
            str += "    min: " + string.Format("{0:0.00}", min) + " + " + string.Format("{0:0.00}", n[minType]) + " (type " + minType + ")" + Environment.NewLine +
                "            " + minChangeAt + Environment.NewLine;
            str += "    max: " + string.Format("{0:0.00}", max) + " + " + string.Format("{0:0.00}", n[maxType]) + " (type " + maxType + ")" + Environment.NewLine +
                "            " + maxChangeAt + Environment.NewLine;
            str += "    ave: " + string.Format("{0:0.00}", ave) + " + " + string.Format("{0:0.00}", shellAve) + Environment.NewLine +
                Environment.NewLine;

            str += "visible:" + Environment.NewLine +
                "    beings: " + visibleBeings.Count + Environment.NewLine +
                "    essence: " + visibleEssence.Count + Environment.NewLine +
                Environment.NewLine;

            str += "feed timer: " + string.Format("{0:0.0}", timer_feed) + "s" + Environment.NewLine +
                "long term visit time: " + string.Format("{0:0}", timer_memory_long_unvisited) + "s" + Environment.NewLine +
                "unsucessful feeds: " + Environment.NewLine +
                "    using short term: " + unsucessful_feeds_short + Environment.NewLine +
                "    using long term: " + unsucessful_feeds_long + Environment.NewLine +
                Environment.NewLine;

            str += "short term memory: " + memory_essence.count_short + Environment.NewLine +
                "long term memory: " + memory_essence.count_long + Environment.NewLine;

            return str;
        }
    }
    class Shell
    {
        public PointF p, velocity, force;
        public float radius, mass;
        internal Collection<Essence> essences;
        internal Being being;
        public int id;

        //public Thread thread;

        public Shell(int _id)
        {
            essences = new Collection<Essence>();
            id = _id;
        }
    }
    class Essence
    {
        public PointF p, velocity, force;
        public float amount, radius, mass;
        internal Shell shell;
        public int type, id;
        public bool external;
        public float timer_creation, age;

        public Thread thread;

        public Essence(int _id)
        {
            external = true;
            id = _id;
        }

        public override string ToString()
        {
            ////if (shell == null)
            ////{
            ////    return "p: " + p;
            ////}
            ////else
            ////{
            ////    return "amount: " + amount;
            ////}

            ////return "Essence: " + protein + " : " + carbo;

            //if (amount == 0)
            //{
            //    return "Protein: " + amount;
            //}
            //else
            //{
            //    return "Carbo: " + amount;
            //}

            return string.Format("{0:0.0}", amount) + " (type " + type + ")";
        }
    }
    class MemoryEssence
    {
        Collection<PointF> ps_short, ps_long;
        Collection<int> ids_short, ids_long;
        public PointF mean_short, mean_long;
        public float sd_short, sd_long, min_spread;
        public int count_short, count_long;

        public int maxMemory_short = 3;
        public int maxMemory_long = 20;

        public MemoryEssence()
        {
            ps_short = new Collection<PointF>();
            ps_long = new Collection<PointF>();
            ids_short = new Collection<int>();
            ids_long = new Collection<int>();
        }

        private void Calculate(bool memory_short, bool memory_long)
        {
            //recalculate averages:
            int c;
            PointF total, subSum, sub, s;

            //short
            if (memory_short & count_short != 0)
            {
                total = new PointF();
                subSum = new PointF();

                c = ps_short.Count;
                for (int i = 0; i < c; i++)
                {
                    total = new PointF(total.X + ps_short[i].X, total.Y + ps_short[i].Y);
                }
                mean_short = new PointF(total.X / (float)c, total.Y / (float)c);

                // s = Sqrt(Sum((X - mean)^2)/(n-1))
                c = ps_short.Count;
                for (int i = 0; i < c; i++)
                {
                    sub = new PointF((ps_short[i].X - mean_short.X) * (ps_short[i].X - mean_short.X), (ps_short[i].Y - mean_short.Y) * (ps_short[i].Y - mean_short.Y));
                    subSum = new PointF(subSum.X + sub.X, subSum.Y + sub.Y);
                }
                if (c <= 1)
                {
                    c = 2;
                }
                s = new PointF((float)Math.Sqrt(subSum.X / (float)(c - 1)), (float)Math.Sqrt(subSum.Y / (float)(c - 1)));
                sd_short = (s.X + s.Y) / 2f;

                if (sd_short < min_spread)
                {
                    sd_short = min_spread;
                }
            }

            //long
            if (memory_long & count_long != 0)
            {
                total = new PointF();
                subSum = new PointF();

                c = ps_long.Count;
                for (int i = 0; i < c; i++)
                {
                    total = new PointF(total.X + ps_long[i].X, total.Y + ps_long[i].Y);
                }
                mean_long = new PointF(total.X / (float)c, total.Y / (float)c);

                // s = Sqrt(Sum((X - mean)^2)/(n-1))
                c = ps_long.Count;
                for (int i = 0; i < c; i++)
                {
                    sub = new PointF((ps_long[i].X - mean_long.X) * (ps_long[i].X - mean_long.X), (ps_long[i].Y - mean_long.Y) * (ps_long[i].Y - mean_long.Y));
                    subSum = new PointF(subSum.X + sub.X, subSum.Y + sub.Y);
                }
                if (c <= 1)
                {
                    c = 2;
                }
                s = new PointF((float)Math.Sqrt(subSum.X / (float)(c - 1)), (float)Math.Sqrt(subSum.Y / (float)(c - 1)));
                sd_long = (s.X + s.Y) / 2f;

                //if (sd_long < min_spread)
                //{
                //    sd_long = min_spread;
                //}
            }
        }

        public void AddMemory(PointF p, int id)
        {
            //make sure that points id doesnt exist in memory
            int c = ids_short.Count;
            for (int i = c - 1; i >= 0; i--)
            {
                if (id == ids_short[i])
                {
                    return;
                }
            }

            //add into memory
            ids_short.Add(id);
            ps_short.Add(p);

            //move to long term memory
            bool changed_old = true;

            if (ids_short.Count > maxMemory_short)
            {
                ids_long.Add(ids_short[0]);
                ps_long.Add(ps_short[0]);

                ids_short.Remove(ids_short[0]);
                ps_short.Remove(ps_short[0]);

                count_long++;
            }
            else
            {
                count_short++;
                changed_old = false;
            }

            //delete old long term memory
            if (ids_long.Count > maxMemory_long)
            {
                ids_long.Remove(ids_long[0]);
                ps_long.Remove(ps_long[0]);

                count_long = maxMemory_long;
            }

            Calculate(true, changed_old);
        }
        public void AddMemory_Long(PointF p, int id)
        {
            ids_long.Add(id);
            ps_long.Add(p);
            count_long++;

            //delete old long term memory
            if (ids_long.Count > maxMemory_long)
            {
                ids_long.Remove(ids_long[0]);
                ps_long.Remove(ps_long[0]);

                count_long = maxMemory_long;
            }
        }

        public void FillShortWithLong()
        {
            Clear_Short();

            if (ids_long.Count != 0)
            {
                AddMemory(mean_long, ids_long[0]);
            }
        }

        public void DumpShortToLong()
        {
            // dump all of short term memory to long term

            if (count_short == 0)
            {
                return;
            }

            int c = count_short;
            for (int i = 0; i < c; i++)
            {
                AddMemory_Long(ps_short[i], ids_short[i]);
            }

            Clear_Short();

            Calculate(false, true);
        }

        public void Clear_Short()
        {
            ps_short.Clear();
            ids_short.Clear();

            count_short = 0;
            mean_short = new PointF();
            sd_short = 0;
        }
        public void Clear_Long()
        {
            ps_long.Clear();
            ids_long.Clear();

            count_long = 0;
            mean_long = new PointF();
            sd_long = 0;
        }

        public override string ToString()
        {
            return "mean: " + mean_short + " sd: " + sd_short;
        }
    }
}
