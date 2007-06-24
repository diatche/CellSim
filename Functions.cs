using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace AI
{
    class F
    {
        public Random random = new Random((int)(DateTime.Now.Ticks));
        float[] z; // z*100
        float[,] tValues;
        float[] tValueSig, tValueDf;
        double INF = double.PositiveInfinity;
        PointF pINF = new PointF(float.PositiveInfinity, float.PositiveInfinity);
        Point origin = new Point();
        v3 v3Origin = new v3();

        float randomCounter;

        public F()
        {
            Set_NormalDistributionValues();
            Set_tValues();
        }

        #region String
        //-----------------------------------------------------------------------

        public string ToString(Array arr, string separator)
        {
            if (arr.Length == 0)
            {
                return "{ }";
            }

            string str = "{";

            int c = arr.Length;
            for (int i = 0; i < c; i++)
            {
                if (i != 0)
                {
                    str += separator;
                }

                str += arr.GetValue(i);
            }

            str += "}";

            return str;
        }
        public string ToString(float[,] frequencies)
        {
            int c = frequencies.GetLength(0);
            if (c == 0)
            {
                return "";
            }
            string str = "";

            for (int i = 0; i < c; i++)
            {
                str += frequencies[i, 0] + " x " + frequencies[i, 1] + Environment.NewLine;
            }

            return str;
        }
        public string ToString(DateTime value, DateTime omitIntersect1, DateTime omitIntersect2, out string omitted)
        {
            bool day, month, year, hour, timeOfDay, minute, second, omitTime;
            string str, dateStr, timeStr, dateOmitted, timeOmitted, tempStr;

            str = "";
            omitted = "";
            dateOmitted = "";
            timeOmitted = "";
            dateStr = "";
            timeStr = "";

            day = true;
            month = true;
            year = true;
            hour = true;
            timeOfDay = true;
            minute = true;
            second = true;

            omitTime = false;

            if (omitIntersect1.Year == omitIntersect2.Year)
            {
                year = false;
                if (omitIntersect1.Month == omitIntersect2.Month)
                {
                    month = false;
                    if (omitIntersect1.Day == omitIntersect2.Day)
                    {
                        day = false;
                        if (omitIntersect1.TimeOfDay == omitIntersect2.TimeOfDay)
                        {
                            timeOfDay = false;
                            if (omitIntersect1.Hour == omitIntersect2.Hour)
                            {
                                hour = false;
                                if (omitIntersect1.Minute == omitIntersect2.Minute)
                                {
                                    minute = false;
                                    if (omitIntersect1.Second == omitIntersect2.Second)
                                    {
                                        second = false;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            string date, time;

            date = Regex.Match(omitIntersect1.ToString(), ".*?( )").Value;
            date = date.Trim();

            time = Regex.Match(omitIntersect1.ToString(), "( ).*").Value;
            time = time.Trim();

            //omit time
            if (hour & timeOfDay & minute & second)
            {
                if (time == "12:00:00 a.m.")
                {
                    omitTime = true;
                }
            }

            //all different
            if (day & month & year & hour & timeOfDay & minute & second)
            {
                return value.ToString();
            }
            if (!day & !month & !year & !hour & !timeOfDay & !minute & !second)
            {
                return value.ToString();
            }

            //date -----

            //day
            if (day)
            {
                if (dateStr == "")
                {
                    dateStr = value.Day.ToString();
                }
                else
                {
                    dateStr += "/" + value.Day;
                }
            }
            else
            {
                if (dateOmitted == "")
                {
                    dateOmitted = value.Day.ToString();
                }
                else
                {
                    dateOmitted += "/" + value.Day;
                }
            }

            //month
            if (month)
            {
                if (dateStr == "")
                {
                    dateStr = value.Month.ToString();
                }
                else
                {
                    dateStr += "/" + value.Month;
                }
            }
            else
            {
                if (dateOmitted == "")
                {
                    dateOmitted = value.Month.ToString();
                }
                else
                {
                    dateOmitted += "/" + value.Month;
                }
            }

            //year
            if (year)
            {
                if (dateStr == "")
                {
                    dateStr = value.Year.ToString();
                }
                else
                {
                    dateStr += "/" + value.Year;
                }
            }
            else
            {
                if (dateOmitted == "")
                {
                    dateOmitted = value.Year.ToString();
                }
                else
                {
                    dateOmitted += "/" + value.Year;
                }
            }

            //time -----
            if (!omitTime)
            {

                //hour
                if (hour)
                {
                    if (timeStr == "")
                    {
                        timeStr = value.Hour.ToString();
                    }
                    else
                    {
                        timeStr += ":" + value.Hour;
                    }
                }
                else
                {
                    if (timeOmitted == "")
                    {
                        timeOmitted = value.Hour.ToString();
                    }
                    else
                    {
                        timeOmitted += ":" + value.Hour;
                    }
                }

                //minute
                if (minute)
                {
                    if (value.Minute.ToString().Length == 1)
                    {
                        tempStr = "0" + value.Minute;
                    }
                    else
                    {
                        tempStr = value.Minute.ToString();
                    }

                    if (timeStr == "")
                    {
                        timeStr = tempStr;
                    }
                    else
                    {
                        timeStr += ":" + tempStr;
                    }
                }
                else
                {
                    if (value.Minute.ToString().Length == 1)
                    {
                        tempStr = "0" + value.Minute;
                    }
                    else
                    {
                        tempStr = value.Minute.ToString();
                    }

                    if (timeOmitted == "")
                    {
                        timeOmitted = tempStr;
                    }
                    else
                    {
                        timeOmitted += ":" + tempStr;
                    }
                }

                //second
                if (second)
                {
                    if (value.Second.ToString().Length == 1)
                    {
                        tempStr = "0" + value.Second;
                    }
                    else
                    {
                        tempStr = value.Second.ToString();
                    }

                    if (timeStr == "")
                    {
                        timeStr = tempStr;
                    }
                    else
                    {
                        timeStr += ":" + tempStr;
                    }
                }
                else
                {
                    if (value.Second.ToString().Length == 1)
                    {
                        tempStr = "0" + value.Second;
                    }
                    else
                    {
                        tempStr = value.Second.ToString();
                    }

                    if (timeOmitted == "")
                    {
                        timeOmitted = tempStr;
                    }
                    else
                    {
                        timeOmitted += ":" + tempStr;
                    }
                }

                ////timeOfDay
                //if (timeOfDay)
                //{
                //    if (timeStr == "")
                //    {
                //        timeStr = value.TimeOfDay.ToString();
                //    }
                //    else
                //    {
                //        timeStr += " " + value.TimeOfDay;
                //    }
                //}
                //else
                //{
                //    if (timeOmitted == "")
                //    {
                //        timeOmitted = value.TimeOfDay.ToString();
                //    }
                //    else
                //    {
                //        timeOmitted += " " + value.TimeOfDay;
                //    }
                //}
            }

            //compile ----

            if (dateStr != "")
            {
                str = dateStr;
                if (timeStr != "")
                {
                    str += " " + timeStr;
                }
            }

            if (dateOmitted != "")
            {
                omitted = dateOmitted;
                if (timeStr != "")
                {
                    omitted += " " + timeOmitted;
                }
            }

            return str;
        }
        public string ToString(DateTime value, DateTime omitIntersect1, DateTime omitIntersect2)
        {
            string s;
            return ToString(value, omitIntersect1, omitIntersect2, out s);
        }

        public int MeasureString(string text, Font font, System.Windows.Forms.PaintEventArgs e)
        {
            return (int)e.Graphics.MeasureString(text, font).Width;
        }
        public int MeasureString(float value, Font font, System.Windows.Forms.PaintEventArgs e)
        {
            //if (value == 0)
            //{
            //    float n = 1;
            //    return MeasureString(n.ToString(), font, e);
            //}
            return MeasureString(value.ToString(), font, e);
        }
        public int MeasureString(int value, Font font, System.Windows.Forms.PaintEventArgs e)
        {
            //if (value == 0)
            //{
            //    float n = 1;
            //    return MeasureString(n.ToString(), font, e);
            //}
            return MeasureString(value.ToString(), font, e);
        }

        //-----------------------------------------------------------------------
        #endregion

        #region Maths
        //-----------------------------------------------------------------------

        public v3 Add(v3 v1, v3 v2)
        {
            return new v3(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
        }
        public PointF Add(PointF v1, PointF v2)
        {
            return new PointF(v1.X + v2.X, v1.Y + v2.Y);
        }
        public Point Add(Point v1, Point v2)
        {
            return new Point(v1.X + v2.X, v1.Y + v2.Y);
        }
        public Collection<object> Add(Collection<object> addTo, Collection<object> itemsToAdd)
        {
            int c = itemsToAdd.Count;
            if (c == 0)
            {
                return addTo;
            }

            for (int i = 0; i < c; i++)
            {
                addTo.Add(itemsToAdd[i]);
            }

            return addTo;
        }
        public float Add(float n1, float n2)
        {
            return n1 + n2;
        }
        public DateTime Add(DateTime addTo, TimeSpan amount)
        {
            return new DateTime(addTo.Ticks + amount.Ticks);
        }

        public v3 Subtract(v3 subtractFrom, v3 subtractBy)
        {
            return new v3(subtractFrom.X - subtractBy.X, subtractFrom.Y - subtractBy.Y, subtractFrom.Z - subtractBy.Z);
        }
        public PointF Subtract(PointF subtractFrom, PointF subtractBy)
        {
            return new PointF(subtractFrom.X - subtractBy.X, subtractFrom.Y - subtractBy.Y);
        }
        public Point Subtract(Point subtractFrom, Point subtractBy)
        {
            return new Point(subtractFrom.X - subtractBy.X, subtractFrom.Y - subtractBy.Y);
        }

        public PointF Dir(PointF p1, PointF p2)
        {
            return new PointF(p2.X - p1.X, p2.Y - p1.Y);
        }

        public v3 Divide(v3 numeratorVector, v3 denominatorVector)
        {
            return new v3(numeratorVector.X / denominatorVector.X,
                numeratorVector.Y / denominatorVector.Y,
                numeratorVector.Z / denominatorVector.Z);
        }
        public v3 Divide(v3 numeratorVector, double denominator)
        {
            return new v3(numeratorVector.X / denominator,
                numeratorVector.Y / denominator,
                numeratorVector.Z / denominator);
        }
        public float Divide(TimeSpan numerator, TimeSpan denominator)
        {
            return numerator.Ticks / denominator.Ticks;
        }
        public TimeSpan Divide(TimeSpan numerator, double denominator)
        {
            return new TimeSpan((long)(numerator.Ticks / denominator));
        }
        public PointF Divide(PointF numeratorVector, float denominator)
        {
            return new PointF(numeratorVector.X / denominator,
                numeratorVector.Y / denominator);
        }
        public float Divide(float numeratorVector, float denominator)
        {
            float n;
            n = numeratorVector / denominator;

            if (float.IsNaN(n))
            {
                return 0;
            }

            return n;
        }

        public PointF Multiply(PointF vector, float multiplier)
        {
            PointF p = new PointF(0, 0);

            p.X = vector.X * multiplier;
            p.Y = vector.Y * multiplier;

            return p;
        }
        public Point Multiply(Point vector, float multiplier)
        {
            Point p = new Point(0, 0);

            p.X = (int)((float)vector.X * multiplier);
            p.Y = (int)((float)vector.Y * multiplier);

            return p;
        }
        public v3 Multiply(v3 vector, double multiplier)
        {
            return new v3(vector.X * multiplier,
    vector.Y * multiplier,
    vector.Z * multiplier);
        }
        public TimeSpan Multiply(TimeSpan timeSpan, double multiplier)
        {
            return new TimeSpan((long)(timeSpan.Ticks * multiplier));
        }

        public v3 Abs(v3 v)
        {
            return new v3(Math.Abs(v.X), Math.Abs(v.Y), Math.Abs(v.Z));
        }
        public int Abs(int n)
        {
            if (n < 0)
            {
                return -n;
            }
            return n;
        }
        public float Abs(float n)
        {
            if (n < 0)
            {
                return -n;
            }
            return n;
        }
        public double Abs(double n)
        {
            if (n < 0)
            {
                return -n;
            }
            return n;
        }

        public double Pow(double n, double power)
        {
            return Math.Pow(n, power);
        }

        public double E(double n, double exponent)
        {
            return n * Math.Pow(10, exponent);
        }

        public float Magnitude(PointF vector)
        {
            return (float)System.Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
        }
        public double Magnitude(v2 vector)
        {
            return System.Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
        }
        public double Magnitude(v3 v)
        {
            return System.Math.Sqrt(v.X * v.X + v.Y * v.Y + v.Z * v.Z);
        }
        public double Magnitude(double x, double y)
        {
            return System.Math.Sqrt(x * x + y * y);
        }
        public double Magnitude(double x, double y, double z)
        {
            return System.Math.Sqrt(x * x + y * y + z * z);
        }
        public float Magnitude(float x, float y)
        {
            return (float)System.Math.Sqrt(x * x + y * y);
        }

        public float Distance(PointF p1, PointF p2)
        {
            float d;
            d = (float)Math.Sqrt((p2.X - p1.X) * (p2.X - p1.X) +
                          (p2.Y - p1.Y) * (p2.Y - p1.Y));
            return d;
        }
        public float Distance(Point p1, Point p2)
        {
            float d;
            d = (float)Math.Sqrt((p2.X - p1.X) * (p2.X - p1.X) +
                          (p2.Y - p1.Y) * (p2.Y - p1.Y));
            return d;
        }
        public double Distance(v2 p1, v2 p2)
        {
            double d;
            d = Math.Sqrt((p2.X - p1.X) * (p2.X - p1.X) +
                          (p2.Y - p1.Y) * (p2.Y - p1.Y));
            return d;
        }
        public double Distance(v3 v1, v3 v2)
        {
            double d;
            d = Math.Sqrt((v2.X - v1.X) * (v2.X - v1.X) +
                          (v2.Y - v1.Y) * (v2.Y - v1.Y) +
                          (v2.Z - v1.Z) * (v2.Z - v1.Z));
            return d;
        }

        public PointF Average(PointF p1, PointF p2)
        {
            return new PointF((p1.X + p2.X) / 2, (p1.Y + p2.Y) / 2);
        }

        public double Bearing(PointF Direction)
        {
            if (Direction == new PointF(0, 0))
            {
                return 0;
            }

            double angle, r;
            PointF A;

            r = Magnitude(Direction);
            A = Direction;

            angle = Math.Asin(A.X / r);

            if (A.Y > 0)
            {
                angle = Math.PI / 2 - angle + Math.PI / 2;
            }

            if (A.X < 0 && A.Y <= 0)
            {
                angle = Math.PI * 2 + angle;
            }

            if (angle > Math.PI * 2)
            {
                angle = Math.PI * 2 - angle;
            }

            return angle;
        }
        public double Bearing(v2 Direction)
        {
            if (Direction == new v2(0, 0))
            {
                return 0;
            }

            double angle, r;
            v2 A;

            r = Magnitude(Direction);
            A = Direction;

            angle = Math.Asin(A.X / r);

            if (A.Y > 0)
            {
                angle = Math.PI / 2 - angle + Math.PI / 2;
            }

            if (A.X < 0 && A.Y <= 0)
            {
                angle = Math.PI * 2 + angle;
            }

            if (angle > Math.PI * 2)
            {
                angle = Math.PI * 2 - angle;
            }

            return angle;
        }

        public float Gradient(PointF p1, PointF p2)
        {
            if (p1 == p2)
            {
                return 0;
            }

            float m;

            m = (p2.Y - p1.Y) / (p2.X - p1.X);

            return m;
        }

        public int NumberDirection(double num)
        {
            if (num == 0)
            {
                return 0;
            }
            else if (num > 0)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }

        public Point FourDirectionalPoint(PointF trueDirection)
        {
            Point p = new Point(0, 0);

            if (trueDirection.X > 0)
            {
                p.X = 1;
            }
            if (trueDirection.X < 0)
            {
                p.X = -1;
            }
            if (trueDirection.Y > 0)
            {
                p.Y = 1;
            }
            if (trueDirection.Y < 0)
            {
                p.Y = -1;
            }

            //Console.WriteLine("Received: " + trueDirection + ". Returned: " + p);

            return p;
        }

        public Point RandomPoint(PointF minInclusive, PointF maxExclusive)
        {
            Point p;
            p = new Point(Random_Next((int)(minInclusive.X), (int)(maxExclusive.X)),
                           Random_Next((int)(minInclusive.Y), (int)(maxExclusive.Y)));

            return p;
        }

        public v3 RandomVector()
        {
            return new v3(Random_Next(-10, 10), Random_Next(-10, 10), Random_Next(-10, 10));
        }

        public PointF RandomPointWithOffset(PointF center, float offset)
        {
            PointF p, rn;
            rn = new PointF(0, 0);

            while (rn == new PointF(0, 0))
            {
                rn.X = Random_Next(-10, 10);
                rn.Y = Random_Next(-10, 10);
            }

            p = Offset(rn, offset);

            p = AddPoints(p, center);

            return p;
        }

        public PointF AddPoints(PointF Vector1, PointF Vector2)
        {
            PointF d;
            d = new PointF(Vector2.X + Vector1.X, Vector2.Y + Vector1.Y);

            return d;
        }
        public Point AddPoints(Point Vector1, Point Vector2)
        {
            Point d;
            d = new Point(Vector2.X + Vector1.X, Vector2.Y + Vector1.Y);

            return d;
        }

        public PointF Opposite(PointF vector)
        {
            PointF v;
            v = new PointF(-vector.X, -vector.Y);

            return v;
        }
        public Point Opposite(Point vector)
        {
            Point v;
            v = new Point(-vector.X, -vector.Y);

            return v;
        }
        public v3 Opposite(v3 vector)
        {
            v3 v;
            v = new v3(-vector.X, -vector.Y, -vector.Z);

            return v;
        }

        public v3 Diff(v3 v1, v3 v2)
        {
            v3 d;
            d = new v3(v2.X - v1.X, v2.Y - v1.Y, v2.Z - v1.Z);

            return d;
        }
        public PointF Diff(PointF Vector1, PointF Vector2)
        {
            PointF d;
            d = new PointF(Vector2.X - Vector1.X, Vector2.Y - Vector1.Y);

            return d;
        }
        public Point Diff(Point Vector1, Point Vector2)
        {
            Point d;
            d = new Point(Vector2.X - Vector1.X, Vector2.Y - Vector1.Y);

            return d;
        }

        public PointF Offset(PointF dir, float dist)
        {
            if (dir == new PointF() | dist == 0)
            {
                return new PointF();
            }

            float d = Magnitude(dir);

            float multiplier = dist / d;
            return new PointF(dir.X * multiplier, dir.Y * multiplier);
        }
        public v3 Offset(v3 dir, double dist)
        {
            if (dir == new v3())
            {
                return new v3();
            }

            double d = Magnitude(dir);

            if (d != 0)
            {
                double multiplier = dist / d;
                v3 v;
                v = new v3(dir.X * multiplier, dir.Y * multiplier, dir.Z * multiplier);


                //Console.WriteLine("dist: " + dist + ". Returned: " + Magnitude3D(v));

                return v;
            }
            else
            {
                return v3Origin;
            }
        }

        public PointF MidPoint(PointF v1, PointF v2)
        {
            return new PointF((v1.X + v2.X) / 2, (v1.Y + v2.Y) / 2);
        }
        public v3 MidPoint(v3 v1, v3 v2)
        {
            return new v3((v1.X + v2.X) / 2, (v1.Y + v2.Y) / 2, (v1.Z + v2.Z) / 2);
        }
        public v3 MidPoint(v3 v)
        {
            return new v3(v.X / 2, v.Y / 2, v.Z / 2);
        }

        public PointF[] MiddlePoint_x2(PointF Vect1, PointF Vect2)
        {
            PointF[] p = new PointF[3];

            p[1] = MidPoint(Vect1, Vect2);
            p[0] = MidPoint(p[1], Vect1);
            p[2] = MidPoint(p[1], Vect2);

            return p;
        }

        public PointF VectorPart(PointF Component, PointF Direction)
        {
            if (Direction == new PointF() | Component == new PointF())
            {
                return new PointF();
            }

            PointF returnValue;
            //Env.Deb.AddVector(New Point(0, 0), New Point(Component.X, Component.Y), "Component", True)
            //Env.Deb.AddVector(New Point(0, 0), New Point(DirectionDiff.X, DirectionDiff.Y), "DirectionDiff", False)


            if (Direction != new PointF(0, 0) && Component != Direction)
            {
                double Angle;

                Angle = AngleBetween(Component, Direction);

                if (Angle != 0)
                {
                    if (Angle != System.Math.PI)
                    {
                        if (Angle != System.Math.PI / 2)
                        {

                            //find VectorPart from SOH
                            double ComponentLength;
                            double VectorPartLength;

                            ComponentLength = Magnitude(Component);
                            VectorPartLength = ComponentLength * System.Math.Cos(Angle);

                            if ((Direction.X == 0 && Direction.Y < 0) || (Direction.X < 0 && Direction.Y == 0))
                            {
                                VectorPartLength = -VectorPartLength;
                            }

                            returnValue = Offset(Direction, (float)VectorPartLength);
                            //End If

                            //Env.Deb.AddVector(New Point(0, 0), New Point(-VectorPart.X, -VectorPart.Y), "-VectorPart", True)

                            //Env.Deb.Vectors.Clear()
                        }
                        else
                        {
                            returnValue = new PointF(0, 0);
                        }
                    }
                    else
                    {
                        returnValue = new PointF(-Component.X, -Component.Y);
                    }
                }
                else
                {
                    returnValue = Component;
                }
            }
            else
            {
                returnValue = Component;
            }

            //Debug.WriteLine("Received Component: " & Component.ToString & ", DirectionDiff: " & DirectionDiff.ToString & ". Returned: " & VectorPart.ToString)
            return returnValue;
        }
        public v3 VectorPart(v3 Component, v3 Direction)
        {
            if (Direction == new v3())
            {
                return new v3();
            }

            v3 returnValue;
            //Env.Deb.AddVector(New Point(0, 0), New Point(Component.X, Component.Y), "Component", True)
            //Env.Deb.AddVector(New Point(0, 0), New Point(DirectionDiff.X, DirectionDiff.Y), "DirectionDiff", False)


            if (Direction != new v3() && Component != Direction)
            {
                double Angle;

                Angle = AngleBetween(Component, Direction);

                if (Angle != 0)
                {
                    if (Angle != System.Math.PI)
                    {
                        if (Angle != System.Math.PI / 2)
                        {

                            //find VectorPart from SOH
                            double ComponentLength;
                            double VectorPartLength;

                            ComponentLength = Magnitude(Component);
                            VectorPartLength = ComponentLength * System.Math.Cos(Angle);

                            //if ((Direction.X == 0 && Direction.Y < 0) || (Direction.X < 0 && Direction.Y == 0))
                            //{
                            //    VectorPartLength = -VectorPartLength;
                            //}

                            returnValue = Offset(Direction, VectorPartLength);
                        }
                        else
                        {
                            returnValue = new v3();
                        }
                    }
                    else
                    {
                        returnValue = new v3(-Component.X, -Component.Y, -Component.Z);
                    }
                }
                else
                {
                    returnValue = Component;
                }
            }
            else
            {
                returnValue = Component;
            }

            //Debug.WriteLine("Received Component: " & Component.ToString & ", DirectionDiff: " & DirectionDiff.ToString & ". Returned: " & VectorPart.ToString)
            return returnValue;
        }

        public PointF CircularMovement_Angle(PointF center, PointF _origin, double angle, bool clockwise)
        {
            double r;
            PointF A;

            A = new PointF(_origin.X - center.X, _origin.Y - center.Y);
            r = Distance(center, _origin);

            if (angle == Math.PI)
            {
                PointF direction = new PointF(-A.X, -A.Y);
                return new PointF(center.X + direction.X, center.Y + direction.Y);
            }
            if (angle == 0)
            {
                return _origin;
            }

            double originAngle, angleTarget;
            PointF B;

            originAngle = -Bearing(A);

            if (clockwise)
            {
                angle = -angle;
            }

            angleTarget = angle + originAngle;
            while (angleTarget > Math.PI * 2)
            {
                angleTarget = Math.PI * 2 - angleTarget;
            }


            B = new PointF(0, 0);
            B.X = (float)(Math.Sin(angleTarget) * r);
            B.Y = (float)(Math.Cos(angleTarget) * r);


            B.X = -B.X;
            B.Y = -B.Y;

            PointF finalB = new PointF(center.X + B.X, center.Y + B.Y);

            return finalB;
        }
        public v2 CircularMovement_Angle(v2 center, v2 _origin, double angle, bool clockwise)
        {
            double r;
            v2 A;

            A = new v2(_origin.X - center.X, _origin.Y - center.Y);
            r = Distance(center, _origin);

            if (angle == Math.PI)
            {
                v2 direction = new v2(-A.X, -A.Y);
                return new v2(center.X + direction.X, center.Y + direction.Y);
            }
            if (angle == 0)
            {
                return _origin;
            }

            double originAngle, angleTarget;
            v2 B;

            originAngle = -Bearing(A);

            if (clockwise)
            {
                angle = -angle;
            }

            angleTarget = angle + originAngle;
            while (angleTarget > Math.PI * 2)
            {
                angleTarget = Math.PI * 2 - angleTarget;
            }


            B = new v2(0, 0);
            B.X = Math.Sin(angleTarget) * r;
            B.Y = Math.Cos(angleTarget) * r;


            B.X = -B.X;
            B.Y = -B.Y;

            v2 finalB = new v2(center.X + B.X, center.Y + B.Y);

            return finalB;
        }

        public PointF CircularMovement_Angle_Diff(double r, double angle, bool clockwise)
        {
            PointF center, _origin, p;

            center = new PointF();
            _origin = new PointF(0, (float)r);

            p = CircularMovement_Angle(center, _origin, angle, clockwise);

            p = Diff(_origin, p);

            return p;
        }

        public PointF CircularMovement_Chord(PointF center, PointF origin, double chordDistance, bool clockwise)
        {
            double r;
            PointF A;

            A = new PointF(origin.X - center.X, origin.Y - center.Y);
            r = Distance(center, origin);

            if (chordDistance >= r * 2)
            {
                PointF direction = new PointF(-A.X, -A.Y);
                direction = Offset(direction, (float)chordDistance);
                return new PointF(origin.X + direction.X, origin.Y + direction.Y);
            }

            double angle, originAngle, angleTarget;
            PointF B;

            angle = 2 * Math.Asin(chordDistance / 2 / r);

            originAngle = -Bearing(A);

            if (clockwise)
            {
                angle = -angle;
            }

            angleTarget = angle + originAngle;
            if (angleTarget > Math.PI * 2)
            {
                angleTarget = Math.PI * 2 - angleTarget;
            }


            B = new PointF(0, 0);
            B.X = (float)(Math.Sin(angleTarget) * r);
            B.Y = (float)(Math.Cos(angleTarget) * r);


            B.X = -B.X;
            B.Y = -B.Y;

            PointF finalB = new PointF(center.X + B.X, center.Y + B.Y);

            return finalB;
        }

        public bool NumberBetween(double num, double lowerInclusive, double upperInclusive)
        {
            if (num >= lowerInclusive && num <= upperInclusive)
            {
                return true;
            }

            return false;
        }

        public bool NumberBetween_Multiple(double[] num, double lowerInclusive, double upperInclusive)
        {
            if (num.Length == 0)
            {
                return false;
            }

            for (int i = 0; i < num.Length; i++)
            {
                if (NumberBetween(num[i], lowerInclusive, upperInclusive) == false)
                {
                    return false;
                }
            }

            return true;
        }

        public bool OddNumber(float num)
        {
            //get number's last digit.
            //divide number by 2.
            //save number as float and as int.
            //if they dont equal then number is odd.
            float nF, nI;
            string lastDigit;
            bool odd = false;

            lastDigit = num + "";
            lastDigit = lastDigit.Substring(lastDigit.Length - 1);

            nF = num / 2;
            nI = (int)(nF);

            if (nF != nI)
            {
                odd = true;
            }

            //Console.WriteLine("num: " + num + ". Odd: " + odd);

            return odd;
        }

        public bool RightOfVector(PointF independentVector, PointF dependentVector)
        {
            bool returnValue;

            //Take perpendicular from vector1 to both sides.
            //The one thats closest determines which side vector2 is on.
            //Right is preferred when 0.

            PointF VectLeft;
            PointF VectRight;
            double DistLeft;
            double DistRight;

            VectLeft = Perpendicular(independentVector, true);
            VectRight = Perpendicular(independentVector, false);

            DistLeft = Distance(dependentVector, VectLeft);
            DistRight = Distance(dependentVector, VectRight);

            if (DistRight <= DistLeft)
            {
                returnValue = true;
            }
            else
            {
                returnValue = false;
            }

            //Debug.WriteLine(Vector1.ToString & ", " & Vector2.ToString & ". ClockwiseOrder: " & ClockwiseOrder)
            return returnValue;
        }
        public bool RightOfVector(v2 independentVector, v2 dependentVector)
        {
            bool returnValue;

            //Take perpendicular from vector1 to both sides.
            //The one thats closest determines which side vector2 is on.
            //Right is preferred when 0.

            v2 VectLeft;
            v2 VectRight;
            double DistLeft;
            double DistRight;

            VectLeft = Perpendicular(independentVector, true);
            VectRight = Perpendicular(independentVector, false);

            DistLeft = Distance(dependentVector, VectLeft);
            DistRight = Distance(dependentVector, VectRight);

            if (DistRight <= DistLeft)
            {
                returnValue = true;
            }
            else
            {
                returnValue = false;
            }

            //Debug.WriteLine(Vector1.ToString & ", " & Vector2.ToString & ". ClockwiseOrder: " & ClockwiseOrder)
            return returnValue;
        }
        public bool RightOfVector(v3 independentVector, v3 dependentVector)
        {
            bool returnValue;

            //Take perpendicular from vector1 to both sides.
            //The one thats closest determines which side vector2 is on.
            //Right is preferred when 0.

            v3 VectLeft;
            v3 VectRight;
            double DistLeft;
            double DistRight;

            VectLeft = Perpendicular(independentVector, dependentVector, true);
            VectRight = Perpendicular(independentVector, dependentVector, false);

            DistLeft = Distance(dependentVector, VectLeft);
            DistRight = Distance(dependentVector, VectRight);

            if (DistRight <= DistLeft)
            {
                returnValue = true;
            }
            else
            {
                returnValue = false;
            }

            //Debug.WriteLine(Vector1.ToString & ", " & Vector2.ToString & ". ClockwiseOrder: " & ClockwiseOrder)
            return returnValue;
        }

        public bool LeftOfVector(PointF independentVector, PointF dependentVector)
        {
            bool b = !RightOfVector(independentVector, dependentVector);
            return b;
        }
        public bool LeftOfVector(v2 independentVector, v2 dependentVector)
        {
            bool b = !RightOfVector(independentVector, dependentVector);
            return b;
        }
        public bool LeftOfVector(v3 independentVector, v3 dependentVector)
        {
            bool b = !RightOfVector(independentVector, dependentVector);
            return b;
        }

        public bool AngleBetweenTwo(double angle, double lowerBound, double upperBound)
        {
            angle = Ang(angle);
            lowerBound = Ang(lowerBound);
            upperBound = Ang(upperBound);

            if (lowerBound == upperBound)
            {
                if (angle == lowerBound)
                {
                    return true;
                }
                return false;
            }
            if (lowerBound < upperBound)
            {
                if (angle >= lowerBound && angle <= upperBound)
                {
                    return true;
                }
                return false;
            }
            else
            {
                if (angle >= lowerBound || angle <= upperBound)
                {
                    return true;
                }
                return false;
            }
        }

        public bool RightOfAngle(double angle1, double angle2)
        {
            double lowerBound, upperBound, angle;

            angle = Ang(angle2);
            lowerBound = Ang(angle1);
            upperBound = OppositeAngle(angle1);

            if (lowerBound == Math.PI)
            {
                if (angle >= lowerBound && angle < Math.PI * 2)
                {
                    return true;
                }
                return false;
            }
            else if (lowerBound < Math.PI)
            {
                if (angle >= lowerBound && angle < upperBound)
                {
                    return true;
                }
                return false;
            }
            else
            {
                if (angle >= lowerBound || angle < upperBound)
                {
                    return true;
                }
                return false;
            }
        }

        public bool ClockwiseOrder_Angle(double angle1, double angle2)
        {
            if (angle2 - angle1 >= 0)
            {
                return true;
            }
            return false;
        }

        public bool ValueBetweenTwo(double value, double lowerInclusive, double upperInclusive)
        {
            if (value > lowerInclusive && value < upperInclusive)
            {
                return true;
            }
            return false;
        }

        public bool PointInsideRectangle(Point p, Rectangle rectangle)
        {
            int x, y;
            x = p.X;
            y = p.Y;

            if (x >= rectangle.X && x <= rectangle.X + rectangle.Width && y >= rectangle.Y && y <= rectangle.Y + rectangle.Height)
            {
                return true;
            }
            return false;
        }
        public bool PointInsideRectangle_WH(PointF pLoc, PointF rLoc, float width, float height)
        {
            float x, y;
            x = pLoc.X;
            y = pLoc.Y;

            if (x >= rLoc.X && x <= rLoc.X + width && y >= rLoc.Y && y <= rLoc.Y + height)
            {
                return true;
            }
            return false;
        }
        public bool PointInsideRectangle_UL(PointF pLoc, PointF lowerLoc, PointF upperLoc)
        {
            float x, y;
            x = pLoc.X;
            y = pLoc.Y;

            if (x >= lowerLoc.X && x <= upperLoc.X && y >= lowerLoc.Y && y <= upperLoc.Y)
            {
                return true;
            }
            return false;
        }

        public PointF PointOfIntersection(PointF[] v1, PointF[] v2)
        {
            if (v1[0] == v1[1] || v2[0] == v2[1])
            {
                return new PointF(float.PositiveInfinity, float.PositiveInfinity);
            }

            float[] m = new float[2];
            float x, y;
            PointF p;

            m[0] = Gradient(v1[0], v1[1]);
            m[1] = Gradient(v2[0], v2[1]);

            if (m[0] == m[1])
            {
                return new PointF(float.PositiveInfinity, float.PositiveInfinity);
            }
            //0 radients
            else if (m[0] == 0)
            {
                //x = (y - y1) / m + x1
                y = v1[0].Y;
                x = (y - v2[0].Y) / m[1] + v2[0].X;

                p = new PointF(x, y);
            }
            else if (m[1] == 0)
            {
                //x = (y - y1) / m + x1
                y = v2[0].Y;
                x = (y - v1[0].Y) / m[0] + v1[0].X;

                p = new PointF(x, y);
            }
            //inf gradients
            else if (m[0] == float.PositiveInfinity)
            {
                //y = m(x - x1) + y1
                x = v1[0].X;
                y = m[1] * (x - v2[0].X) + v2[0].Y;

                p = new PointF(x, y);
            }
            else if (m[1] == float.PositiveInfinity)
            {
                //y = m(x - x1) + y1
                x = v2[0].X;
                y = m[0] * (x - v1[0].X) + v1[0].Y;

                p = new PointF(x, y);
            }
            else
            {
                //y - y1 = m(x - x1)

                y = (-v2[0].Y + m[1] * (v2[0].X - v1[0].X + v1[0].Y / m[0])) / (m[1] / m[0] - 1);
                x = (y - v1[0].Y) / m[0] + v1[0].X;

                p = new PointF(x, y);
            }

            return p;
        }

        public bool PointOfIntersectionWithinBounds(PointF[] v1, PointF[] v2)
        {
            PointF p;
            bool b;

            p = PointOfIntersection(v1, v2);


            //if (env.db != null)
            //{
            //    env.db.AddVector(origin, p, "", false);
            //}

            if (p == pINF)
            {
                return false;
            }

            b = true;

            //v1:
            //work out diff to determine how the bounds work

            PointF diff;
            diff = Diff(v1[0], v1[1]);

            //horizontal
            if (diff.Y == 0)
            {
                if ((p.X <= v1[0].X && p.X >= v1[1].X) || (p.X >= v1[0].X && p.X <= v1[1].X))
                {
                }
                else
                {
                    b = false;
                }
            }
            //vertical
            else if (diff.X == 0)
            {
                if ((p.Y <= v1[0].Y && p.Y >= v1[1].Y) || (p.Y >= v1[0].Y && p.Y <= v1[1].Y))
                {
                }
                else
                {
                    b = false;
                }
            }
            //--,++
            else if (diff.X > 0 & diff.Y > 0)
            {
                if (p.X >= v1[0].X && p.X <= v1[1].X && p.Y >= v1[0].Y && p.Y <= v1[1].Y)
                {
                }
                else
                {
                    b = false;
                }
            }
            //++,--
            else if (diff.X < 0 & diff.Y < 0)
            {
                if (p.X <= v1[0].X && p.X >= v1[1].X && p.Y <= v1[0].Y && p.Y >= v1[1].Y)
                {
                }
                else
                {
                    b = false;
                }
            }
            //+-,-+
            else if (diff.X > 0 & diff.Y < 0)
            {
                if (p.X >= v1[0].X && p.X <= v1[1].X && p.Y <= v1[0].Y && p.Y >= v1[1].Y)
                {
                }
                else
                {
                    b = false;
                }
            }
            //-+,+-
            else if (diff.X < 0 & diff.Y > 0)
            {
                if (p.X <= v1[0].X && p.X >= v1[1].X && p.Y >= v1[0].Y && p.Y <= v1[1].Y)
                {
                }
                else
                {
                    b = false;
                }
            }

            //v2:
            //work out diff to determine how the bounds work

            diff = Diff(v2[0], v2[1]);

            //horizontal
            if (diff.Y == 0)
            {
                if ((p.X <= v2[0].X && p.X >= v2[1].X) || (p.X >= v2[0].X && p.X <= v2[1].X))
                {
                }
                else
                {
                    b = false;
                }
            }
            //vertical
            else if (diff.X == 0)
            {
                if ((p.Y <= v2[0].Y && p.Y >= v2[1].Y) || (p.Y >= v2[0].Y && p.Y <= v2[1].Y))
                {
                }
                else
                {
                    b = false;
                }
            }
            //--,++
            else if (diff.X > 0 & diff.Y > 0)
            {
                if (p.X >= v2[0].X && p.X <= v2[1].X && p.Y >= v2[0].Y && p.Y <= v2[1].Y)
                {
                }
                else
                {
                    b = false;
                }
            }
            //++,--
            else if (diff.X < 0 & diff.Y < 0)
            {
                if (p.X <= v2[0].X && p.X >= v2[1].X && p.Y <= v2[0].Y && p.Y >= v2[1].Y)
                {
                }
                else
                {
                    b = false;
                }
            }
            //+-,-+
            else if (diff.X > 0 & diff.Y < 0)
            {
                if (p.X >= v2[0].X && p.X <= v2[1].X && p.Y <= v2[0].Y && p.Y >= v2[1].Y)
                {
                }
                else
                {
                    b = false;
                }
            }
            //-+,+-
            else if (diff.X < 0 & diff.Y > 0)
            {
                if (p.X <= v2[0].X && p.X >= v2[1].X && p.Y >= v2[0].Y && p.Y <= v2[1].Y)
                {
                }
                else
                {
                    b = false;
                }
            }

            return b;
        }

        public bool LineCrossesRectangle_UL(PointF p1, PointF p2, PointF lowerLoc, PointF upperLoc)
        {
            /// <summary>
            /// if the line crosses any of the rectangle lines, return true.
            /// </summary>

            PointF[] v1, v2;
            bool b = false;

            v1 = new PointF[2];
            v2 = new PointF[2];

            v1[0] = p1;
            v1[1] = p2;

            //top
            v2[0] = lowerLoc;
            v2[1] = new PointF(upperLoc.X, lowerLoc.Y);

            //if (env.db != null)
            //{
            //    env.db.AddVector(v2[0], v2[1], "", false);
            //}

            if (PointOfIntersectionWithinBounds(v1, v2))
            {
                b = true;
            }

            //right
            v2[0] = v2[1];
            v2[1] = upperLoc;

            //if (env.db != null)
            //{
            //    env.db.AddVector(v2[0], v2[1], "", false);
            //}

            if (PointOfIntersectionWithinBounds(v1, v2))
            {
                b = true;
            }

            //bot
            v2[0] = v2[1];
            v2[1] = new PointF(lowerLoc.X, upperLoc.Y);

            //if (env.db != null)
            //{
            //    env.db.AddVector(v2[0], v2[1], "", false);
            //}

            if (PointOfIntersectionWithinBounds(v1, v2))
            {
                b = true;
            }

            //left
            v2[0] = v2[1];
            v2[1] = lowerLoc;

            //if (env.db != null)
            //{
            //    env.db.AddVector(v2[0], v2[1], "", false);
            //}

            if (PointOfIntersectionWithinBounds(v1, v2))
            {
                b = true;
            }

            //if (env.db != null)
            //{
            //    env.db.AddVector(v1[0], v1[1], "intersect: " + b, false);
            //}

            return b;
        }

        public bool PointOnBoundsOfRectangle_UL(PointF pLoc, PointF lowerLoc, PointF upperLoc)
        {
            float x, y;
            x = pLoc.X;
            y = pLoc.Y;

            //vert.
            if ((x == lowerLoc.X || x == upperLoc.X) && (y >= lowerLoc.Y && y <= upperLoc.Y))
            {
                return true;
            }
            //horiz.
            if ((y == lowerLoc.Y || y == upperLoc.Y) && (x >= lowerLoc.X && x <= upperLoc.X))
            {
                return true;
            }
            return false;
        }

        public bool IsInf(double n)
        {
            if (n == double.PositiveInfinity)
            {
                return true;
            }
            else if (n == double.NegativeInfinity)
            {
                return true;
            }
            return false;
        }

        public bool Perpendicular(v2 v1, v2 v2)
        {
            if (DotProduct(v1, v2) == 0)
            {
                return true;
            }
            return false;
        }
        public v2 Perpendicular(v2 Vector, bool Left)
        {
            v2 v = new v2();

            if (Left)
            {
                v.X = Vector.Y;
                v.Y = -Vector.X;
            }
            else
            {
                v.X = -Vector.Y;
                v.Y = Vector.X;
            }
            return v;
        }
        public PointF Perpendicular(PointF Vector, bool Left)
        {
            PointF v = new PointF(0, 0);

            if (Left)
            {
                v.X = Vector.Y;
                v.Y = -Vector.X;
            }
            else
            {
                v.X = -Vector.Y;
                v.Y = Vector.X;
            }
            return v;
        }
        public Point Perpendicular(Point Vector, bool Left)
        {
            Point v = new Point(0, 0);

            if (Left)
            {
                v.X = Vector.Y;
                v.Y = -Vector.X;
            }
            else
            {
                v.X = -Vector.Y;
                v.Y = Vector.X;
            }
            return v;
        }
        public v3 Perpendicular(v3 perpendicularTo, v3 vectorOnPlane, bool left)
        {
            v3 perp1, perp2;

            perp1 = CrossProduct(perpendicularTo, vectorOnPlane);
            perp2 = CrossProduct(perp1, perpendicularTo);

            return perp2;
        }

        public double TicksToSeconds(long ticks)
        {
            return ticks / 10000000.0;
        }
        public double TicksToDays(long ticks)
        {
            return ticks / 864000000000.0;
        }

        public double DotProduct(v2 v1, v2 v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y;
        }

        public v3 Normalise(v3 v)
        {
            //v = Abs(v); //||

            double div;
            v3 newV;
            //div = HighestAbsoluteComponent(v);
            div = Magnitude(v);

            if (div == 1)
            {
                return v;
            }

            newV = new v3(v.X / div, v.Y / div, v.Z / div);

            //Console.WriteLine("[NormaliseDirection] v: " + v + ". Returned: " + newV.ToString());

            return newV;
        }
        public PointF Normalise(PointF v)
        {
            //v = Abs(v); //||

            float div;
            PointF newV;
            //div = HighestAbsoluteComponent(v);
            div = Magnitude(v);

            if (div == 1)
            {
                return v;
            }

            newV = new PointF(v.X / div, v.Y / div);

            //Console.WriteLine("[NormaliseDirection] v: " + v + ". Returned: " + newV.ToString());

            return newV;
        }

        public void TwoPerpendicularVectors(v3 v1in, v3 v2in, out v3 v1out, out v3 v2out)
        {
            //get normal of the two, then get normal of v1 and the calculated normal

            v3 norm1;

            norm1 = CrossProduct(v1in, v2in);
            v2in = CrossProduct(norm1, v1in);

            v1out = v1in;
            v2out = v2in;
        }

        public v3 CrossProduct(v3 u, v3 v)
        {
            v3 r = new v3();

            r.X = u.Y * v.Z - u.Z * v.Y;
            r.Y = -u.X * v.Z + u.Z * v.X;
            r.Z = u.X * v.Y - u.Y * v.X;

            return r;
        }

        public double HighestAbsoluteComponent(v3 v)
        {
            double highest = 0;
            v3 absV = Abs(v);

            //X
            highest = absV.X;

            //Y
            if (absV.Y > highest)
            {
                highest = absV.Y;
            }

            //Z
            if (absV.Z > highest)
            {
                highest = absV.Z;
            }

            //Console.WriteLine("[HighestAbsoluteComponent] v: " + v + ". Returned: " + highest);

            return highest;
        }

        public Color ColorFromRGB(double r, double g, double b)
        {
            int R, G, B;
            R = (int)(NumberBound(r, 0, 255));
            G = (int)(NumberBound(g, 0, 255));
            B = (int)(NumberBound(b, 0, 255));

            return Color.FromArgb(R, G, B);
        }

        public v3 ConvertToAngle(v3 v)
        {
            //y:pitch, z:roll

            //pitch: +y axis towards +z axis
            //roll: +y axis towards -x axis

            //using AngleBetween(v3,v3), get angle between axis and
            //flattened vector

            v3 angle;
            v2 axis, loc;

            angle = new v3();
            axis = new v2(0, 1);

            //pitch
            loc = new v2(v.Z, v.Y);
            angle.Y = AngleBetween_Clockwise(axis, loc);

            //roll
            loc = new v2(v.X, v.Y);
            angle.Z = AngleBetween_Clockwise(axis, loc);

            //Console.WriteLine("[AngleBetween_Clockwise] v: " + v.ToString() + ". Angle: " + angle.ToString());

            return angle;

            //v3 angle;
            //v2 axis, loc;

            //angle = new v3();
            //axis = new v2(0, 1);

            ////yaw
            //loc = new v2(v.X, v.Z);
            //angle.X = AngleBetween_Clockwise(axis, loc);

            ////pitch
            //loc = new v2(v.Z, v.Y);
            //angle.Y = AngleBetween_Clockwise(axis, loc);

            ////roll
            //loc = new v2(v.X, v.Y);
            //angle.Z = AngleBetween_Clockwise(axis, loc);

            ////Console.WriteLine("[AngleBetween_Clockwise] v: " + v.ToString() + ". Angle: " + angle.ToString());

            //return angle;

        }

        public double NumberBound(double n, double lowerBound, double upperBound)
        {
            //Console.Write("n: " + n);

            if (lowerBound != double.NegativeInfinity)
            {
                if (n < lowerBound)
                {
                    n = lowerBound;
                }
            }
            if (upperBound != double.PositiveInfinity)
            {
                if (n > upperBound)
                {
                    n = upperBound;
                }
            }

            //Console.WriteLine(", lowerBound: " + lowerBound + ", upperBound: " + upperBound + ". Returned: " + n);

            return n;
        }
        public float NumberBound(float n, float lowerBound, float upperBound)
        {
            //Console.Write("n: " + n);

            if (lowerBound != float.NegativeInfinity)
            {
                if (n < lowerBound)
                {
                    n = lowerBound;
                }
            }
            if (upperBound != float.PositiveInfinity)
            {
                if (n > upperBound)
                {
                    n = upperBound;
                }
            }

            //Console.WriteLine(", lowerBound: " + lowerBound + ", upperBound: " + upperBound + ". Returned: " + n);

            return n;
        }
        public int NumberBound(int n, int lowerBound, int upperBound)
        {
            if (n < lowerBound)
            {
                n = lowerBound;
            }
            if (n > upperBound)
            {
                n = upperBound;
            }

            return n;
        }
        public int NumberBound(int n, int lowerBound)
        {
            if (n < lowerBound)
            {
                n = lowerBound;
            }
            return n;
        }
        public float NumberBound(float n, float lowerBound)
        {
            if (n < lowerBound)
            {
                n = lowerBound;
            }
            return n;
        }

        public void Trig_abc(out double AorB, double c)
        {
            //a^2 + b^2 = c^2
            //2a^2 = c^2

            AorB = c / Math.Sqrt(2);
        }
        public void Trig_abc(out double adjacent, out double opposite, double hypotenuse, double angle)
        {
            //sin(angle) = opposite/hypotenuse
            opposite = Math.Sin(angle) * hypotenuse;

            //a^2 + b^2 = c^2
            adjacent = Math.Sqrt(hypotenuse * hypotenuse - opposite * opposite);
        }

        public double Gradient(PointF p)
        {
            double m;
            m = p.Y / p.X;

            return m;
        }

        public float AngleBetween(PointF dir1, PointF dir2)
        {
            float a;
            //a.b = |a|.|b|.cos(Angle)

            double ScalarProduct;
            double Magnitude1;
            double Magnitude2;

            ScalarProduct = (dir1.X * dir2.X) + (dir1.Y * dir2.Y);
            Magnitude1 = System.Math.Sqrt((dir1.X * dir1.X) + (dir1.Y * dir1.Y));
            Magnitude2 = System.Math.Sqrt((dir2.X * dir2.X) + (dir2.Y * dir2.Y));

            if (Magnitude1 == 0 | Magnitude2 == 0)
            {
                return 0;
            }


            a = (float)System.Math.Acos(ScalarProduct / Magnitude1 / Magnitude2);

            //Debug.Write("Angle: " & AngleBetween & " (" & AngleBetween / PI * 180 & ")")
            return a;
        }
        public double AngleBetween(v2 dir1, v2 dir2)
        {
            double a;
            //a.b = |a|.|b|.cos(Angle)

            double ScalarProduct;
            double Magnitude1;
            double Magnitude2;

            ScalarProduct = (dir1.X * dir2.X) + (dir1.Y * dir2.Y);
            Magnitude1 = System.Math.Sqrt((dir1.X * dir1.X) + (dir1.Y * dir1.Y));
            Magnitude2 = System.Math.Sqrt((dir2.X * dir2.X) + (dir2.Y * dir2.Y));

            if (Magnitude1 == 0 | Magnitude2 == 0)
            {
                return 0;
            }

            a = System.Math.Acos(ScalarProduct / Magnitude1 / Magnitude2);

            //Debug.Write("Angle: " & AngleBetween & " (" & AngleBetween / PI * 180 & ")")
            return a;
        }
        public double AngleBetween(v3 dir1, v3 dir2)
        {
            double a;
            //a.b = |a|.|b|.cos(Angle)

            double ScalarProduct;
            double Magnitude1;
            double Magnitude2;

            ScalarProduct = (dir1.X * dir2.X) + (dir1.Y * dir2.Y) + (dir1.Z * dir2.Z);
            Magnitude1 = System.Math.Sqrt((dir1.X * dir1.X) + (dir1.Y * dir1.Y) + (dir1.Z * dir1.Z));
            Magnitude2 = System.Math.Sqrt((dir2.X * dir2.X) + (dir2.Y * dir2.Y) + (dir2.Z * dir2.Z));

            if (Magnitude1 == 0 | Magnitude2 == 0)
            {
                return 0;
            }

            a = System.Math.Acos(ScalarProduct / Magnitude1 / Magnitude2);

            //Debug.Write("Angle: " & AngleBetween & " (" & AngleBetween / PI * 180 & ")")
            return a;
        }

        public double AngleBetween_Clockwise(v2 dir1, v2 dir2)
        {
            double angle;

            angle = AngleBetween(dir1, dir2);

            if (!LeftOfVector(dir1, dir2))
            {
                angle += Math.PI;
            }

            //Console.WriteLine("[AngleBetween_Clockwise] dir1: " + dir1 + ", dir2: " + dir2 + ". Angle: " + angle);

            return angle;

        }

        public double RandomAngle()
        {
            double n;

            n = Random_NextDouble() * Math.PI * 2;

            //Console.WriteLine("RandomAngle: " + (n / Math.PI * 180));

            return n;
        }

        public double Ang(double angle)
        {
            double a;
            a = angle;

            if (a >= Math.PI * 2)
            {
                a = a - Math.PI * 2;
            }

            if (a < 0)
            {
                a = Math.PI + a;
            }

            return a;
        }

        public double PolarSqrt(double value)
        {
            double n;
            n = Math.Sqrt(Math.Abs(value));

            if (value < 0)
            {
                n = -n;
            }
            return n;
        }

        public double ToRadians(int degrees)
        {
            return degrees / 180 * Math.PI;
        }

        public double OppositeAngle(double angle)
        {
            double a;
            a = angle + Math.PI;

            a = Ang(a);

            return a;
        }

        public float Closest(float[] values, float value, out int index)
        {
            //use difference to find closest.

            index = 0;

            int c = values.Length;
            if (c == 0)
            {
                return 0;
            }

            float dist, closestDist, closestValue;
            closestDist = float.MaxValue;
            closestValue = 0;
            index = 0;

            for (int i = 0; i < c; i++)
            {
                dist = (float)(Math.Abs(value - values[i]));
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closestValue = values[i];
                    index = i;
                }
            }

            return closestValue;
        }

        public double[] Sort(double[] array, bool ascending)
        {
            int c = array.Length;
            if (c == 0)
            {
                return array;
            }

            int i = 0;
            int v = 0;
            int index = 0;
            double[] newArray = new double[array.Length];

            for (i = 0; i < c; i++)
            {

                index = array.Length - 1;

                for (v = 0; v < c; v++)
                {
                    if (ascending)
                    {
                        if (v != i & array[v] > array[i])
                        {
                            index--;
                        }
                    }
                    else
                    {
                        if (v != i & array[v] < array[i])
                        {
                            index--;
                        }
                    }
                }
                newArray[index] = array[i];

                //find repeats
                for (v = 0; v < c; v++)
                {
                    if (v != i & array[v] == array[i])
                    {
                        index--;
                        newArray[index] = array[i];
                    }
                }
            }

            return newArray;
        }
        public float[] Sort(float[] array, bool ascending)
        {
            int c = array.Length;
            if (c == 0)
            {
                return array;
            }

            int i = 0;
            int v = 0;
            int index = 0;
            float[] newArray = new float[array.Length];

            for (i = 0; i < c; i++)
            {

                index = array.Length - 1;

                for (v = 0; v < c; v++)
                {
                    if (ascending)
                    {
                        if (v != i & array[v] > array[i])
                        {
                            index--;
                        }
                    }
                    else
                    {
                        if (v != i & array[v] < array[i])
                        {
                            index--;
                        }
                    }
                }
                newArray[index] = array[i];

                //find repeats
                for (v = 0; v < c; v++)
                {
                    if (v != i & array[v] == array[i])
                    {
                        index--;
                        newArray[index] = array[i];
                    }
                }
            }

            return newArray;
        }
        public int[] Sort(int[] array, bool ascending)
        {
            int c = array.Length;
            if (c == 0)
            {
                return array;
            }

            int i = 0;
            int v = 0;
            int index = 0;
            int[] newArray = new int[array.Length];

            for (i = 0; i < c; i++)
            {

                index = array.Length - 1;

                for (v = 0; v < c; v++)
                {
                    if (ascending)
                    {
                        if (v != i & array[v] > array[i])
                        {
                            index--;
                        }
                    }
                    else
                    {
                        if (v != i & array[v] < array[i])
                        {
                            index--;
                        }
                    }
                }
                newArray[index] = array[i];

                //find repeats
                for (v = 0; v < c; v++)
                {
                    if (v != i & array[v] == array[i])
                    {
                        index--;
                        newArray[index] = array[i];
                    }
                }
            }

            return newArray;
        }
        public DateTime[] Sort(DateTime[] array, bool ascending)
        {
            int c = array.Length;
            if (c == 0)
            {
                return array;
            }

            int i = 0;
            int v = 0;
            int index = 0;
            DateTime[] newArray = new DateTime[array.Length];

            for (i = 0; i < c; i++)
            {

                index = array.Length - 1;

                for (v = 0; v < c; v++)
                {
                    if (ascending)
                    {
                        if (v != i & array[v] > array[i])
                        {
                            index--;
                        }
                    }
                    else
                    {
                        if (v != i & array[v] < array[i])
                        {
                            index--;
                        }
                    }
                }
                newArray[index] = array[i];

                //find repeats
                for (v = 0; v < c; v++)
                {
                    if (v != i & array[v] == array[i])
                    {
                        index--;
                        newArray[index] = array[i];
                    }
                }
            }

            return newArray;
        }

        public float[] Recreate(float[] values)
        {
            int c = values.Length;
            if (c == 0)
            {
                return null;
            }

            float[] newValues = new float[c];

            for (int i = 0; i < c; i++)
            {
                newValues[i] = values[i];
            }

            return newValues;
        }
        public int[] Recreate(int[] values)
        {
            int c = values.Length;
            if (c == 0)
            {
                return null;
            }

            int[] newValues = new int[c];

            for (int i = 0; i < c; i++)
            {
                newValues[i] = values[i];
            }

            return newValues;
        }
        public DateTime[] Recreate(DateTime[] values)
        {
            int c = values.Length;
            if (c == 0)
            {
                return null;
            }

            DateTime[] newValues = new DateTime[c];

            for (int i = 0; i < c; i++)
            {
                newValues[i] = values[i];
            }

            return newValues;
        }

        public float Scale(float value, float oldScale, float newScale)
        {
            return value / oldScale * newScale;
        }

        //public int MaxValue(int value1, int value2)
        //{
        //    if (value1 > value2)
        //    {
        //        return value1;
        //    }
        //    return value2;
        //}
        //public float MaxValue(float[] values)
        //{
        //    if (values == null)
        //    {
        //        return 0;
        //    }

        //    values = Sort(values);

        //    return values[values.Length - 1];
        //}
        //public int MaxValue(int[] values)
        //{
        //    if (values == null)
        //    {
        //        return 0;
        //    }

        //    values = Sort(values);

        //    return values[values.Length - 1];
        //}
        //public DateTime MaxValue(DateTime[] values)
        //{
        //    if (values == null)
        //    {
        //        return new DateTime();
        //    }

        //    values = Sort(values);

        //    return values[values.Length - 1];
        //}

        //public float MinValue(float[] values)
        //{
        //    if (values == null)
        //    {
        //        return 0;
        //    }

        //    values = Sort(values);

        //    return values[0];
        //}
        //public int MinValue(int[] values)
        //{
        //    if (values == null)
        //    {
        //        return 0;
        //    }

        //    values = Sort(values);

        //    return values[0];
        //}
        //public DateTime MinValue(DateTime[] values)
        //{
        //    if (values == null)
        //    {
        //        return new DateTime();
        //    }

        //    values = Sort(values);

        //    return values[0];
        //}
        public float MinValue(float value1, float value2)
        {
            if (value2 < value1)
            {
                return value2;
            }
            return value1;
        }

        public void MinMaxValue(float[] values, out float min, out float max)
        {
            int c = values.Length;
            if (c == 0)
            {
                min = 0;
                max = 0;

                return;
            }

            max = float.MinValue;
            min = float.MaxValue;


            for (int i = 0; i < c; i++)
            {
                if (values[i] > max)
                {
                    max = values[i];
                }
                if (values[i] < min)
                {
                    min = values[i];
                }
            }
        }
        public void MinMaxValue(int[] values, out int min, out int max)
        {
            int c = values.Length;
            if (c == 0)
            {
                min = 0;
                max = 0;

                return;
            }

            max = int.MinValue;
            min = int.MaxValue;


            for (int i = 0; i < c; i++)
            {
                if (values[i] > max)
                {
                    max = values[i];
                }
                if (values[i] < min)
                {
                    min = values[i];
                }
            }
        }
        public void MinMaxValue(double[] values, out double min, out double max)
        {
            int c = values.Length;
            if (c == 0)
            {
                min = 0;
                max = 0;

                return;
            }

            max = double.MinValue;
            min = double.MaxValue;


            for (int i = 0; i < c; i++)
            {
                if (values[i] > max)
                {
                    max = values[i];
                }
                if (values[i] < min)
                {
                    min = values[i];
                }
            }
        }
        public void MinMaxValue(DateTime[] values, out DateTime min, out DateTime max)
        {
            int c = values.Length;
            if (c == 0)
            {
                min = new DateTime();
                max = new DateTime();

                return;
            }

            max = DateTime.MinValue;
            min = DateTime.MaxValue;


            for (int i = 0; i < c; i++)
            {
                if (values[i] > max)
                {
                    max = values[i];
                }
                if (values[i] < min)
                {
                    min = values[i];
                }
            }
        }

        public float CleanValue(float value)
        {
            string str;
            //int n;

            str = value.ToString();
            value = Convert.ToSingle(str);

            //if (str.Contains("0000"))
            //{
            //    n = str.IndexOf("0000");
            //    value = (float)Math.Round(value, n);
            //}

            return value;
        }
        public double CleanValue(double value)
        {
            string str;
            //int n;

            str = value.ToString();
            value = Convert.ToDouble(str);

            //if (str.Contains("0000"))
            //{
            //    n = str.IndexOf("0000");
            //    value = Math.Round(value, n);
            //}

            return value;
        }

        public double RoundToNearest(double num, double toNearest)
        {
            num /= toNearest;
            num = Math.Round(num);
            num *= toNearest;

            num = CleanValue(num);

            return num;
        }
        public float RoundToNearest(float num, float toNearest)
        {
            num /= toNearest;
            num = (float)Math.Round(num);
            num *= toNearest;

            num = CleanValue(num);

            return num;
        }
        public float[] RoundToNearest(float[] _values, float toNearest)
        {
            int c = _values.Length;
            if (c == 0)
            {
                return null;
            }

            float[] values = _values;

            for (int i = 0; i < c; i++)
            {
                values[i] /= toNearest;
                values[i] = (float)Math.Round(values[i]);
                values[i] *= toNearest;

                values[i] = CleanValue(values[i]);
            }

            return values;
        }

        public float RoundToSigFig_Nearest(float num, float sigFig)
        {
            float multiplier;
            int count;

            if (num < 1)
            {
                multiplier = 10f;
            }
            else
            {
                multiplier = 0.1f;
            }

            count = 0;

            //multiply num until it is < 10 and >= 1
            while (num >= 10 | num < 1)
            {
                num *= multiplier;
                count++;
            }

            //round and return to original sig.fig.
            num = RoundToNearest(num, sigFig);
            num = NumberBound(num, 1);
            num = num / (float)Math.Pow(multiplier, count);

            //fix number
            if (num > 1)
            {
                num = (float)Math.Round(num);
            }

            return num;
        }

        public float RoundToSigFig_Ceiling(float num, float sigFig)
        {
            float multiplier;
            int count;

            if (num < 1)
            {
                multiplier = 10f;
            }
            else
            {
                multiplier = 0.1f;
            }

            count = 0;

            //multiply num until it is < 10 and >= 1
            while (num >= 10 | num < 1)
            {
                num *= multiplier;
                count++;
            }

            //round and return to original sig.fig.
            num = RoundToCeiling(num, sigFig);
            num = NumberBound(num, 1);
            num = num / (float)Math.Pow(multiplier, count);

            //fix number
            if (num > 1)
            {
                num = (float)Math.Round(num);
            }

            return num;
        }

        public float RoundToCeiling(float num, float toNearest)
        {
            num /= toNearest;
            num = (float)Math.Ceiling(num);
            num *= toNearest;

            num = CleanValue(num);

            return num;
        }

        public Frequency Frequencies(float[] _values, float interval)
        {
            int c = _values.Length;
            if (c == 0)
            {
                return null;
            }

            MedianSummary summary;
            float min, max, value;
            float[] rawValues, values;
            int[] freq;
            bool found;
            int count, inc;

            rawValues = _values;
            rawValues = Sort(rawValues, true);
            rawValues = RoundToNearest(rawValues, interval);
            summary = Summary(rawValues);
            min = summary.min;
            max = summary.max;
            count = 0;
            inc = 0;

            //count number of different values
            for (value = min; value <= max; value += interval)
            {
                value = CleanValue(value);

                found = false;
                for (int i = 0; i < c; i++)
                {
                    if (rawValues[i] == value)
                    {
                        found = true;
                        break;
                    }
                }

                if (found)
                {
                    count++;
                }
            }

            //create array
            freq = new int[count];
            values = new float[count];

            for (value = min; value <= max; value += interval)
            {
                value = CleanValue(value);

                count = 0;
                for (int i = 0; i < c; i++)
                {
                    if (rawValues[i] == value)
                    {
                        count++;
                    }
                }

                if (count != 0)
                {
                    values[inc] = value;
                    freq[inc] = count;
                    inc++;
                }
            }

            return new Frequency(values, freq, interval);
        }
        public Frequency Frequencies(float[] values)
        {
            float interval, sd;

            sd = SD(values);

            interval = sd / 3f;
            interval = RoundToSigFig_Nearest(interval, 5);

            return Frequencies(values, interval);
        }

        public float[] FrequenciesToValues(float[] values, float[] frequency)
        {
            //get total
            int n, count;
            float[] final;
            n = (int)Sum(frequency);

            final = new float[n];
            count = 0;

            int c = values.Length;
            for (int i = 0; i < c; i++)
            {
                if (frequency[i] != 0)
                {
                    for (int v = 1; v <= frequency[i]; v++)
                    {
                        final[count] = values[i];
                        count++;
                    }
                }
            }
            return final;
        }
        public float[] FrequenciesToValues(Frequency frequencies)
        {
            float[] values;
            int[] frequency;

            values = frequencies.values;
            frequency = frequencies.frequencies;

            //get total
            int n, count;
            float[] final;
            n = Sum(frequency);

            final = new float[n];
            count = 0;

            int c = values.Length;
            for (int i = 0; i < c; i++)
            {
                if (frequency[i] != 0)
                {
                    for (int v = 1; v <= frequency[i]; v++)
                    {
                        final[count] = values[i];
                        count++;
                    }
                }
            }
            return final;
        }

        public void OmitZeros(float[] values, DateTime[] partnerValues, out float[] newValues, out DateTime[] newPartnerValues)
        {
            newValues = null;
            newPartnerValues = null;

            int c = values.Length;
            if (c == 0 || c != partnerValues.Length)
            {
                return;
            }

            int count = 0;

            //count non zeros
            for (int i = 0; i < c; i++)
            {
                if (values[i] != 0)
                {
                    count++;
                }
            }

            newValues = new float[count];
            newPartnerValues = new DateTime[count];

            count = 0;

            //add data omiting zeros
            for (int i = 0; i < c; i++)
            {
                if (values[i] != 0)
                {
                    newValues[count] = values[i];
                    newPartnerValues[count] = partnerValues[i];
                    count++;
                }
            }
        }

        //-----------------------------------------------------------------------
        #endregion

        #region Stats
        //-----------------------------------------------------------------------

        public MedianSummary Summary(float[] values)
        {
            int c = values.Length;
            if (c == 0)
            {
                return null;
            }

            MedianSummary summary = new MedianSummary();

            values = Sort(values, true);

            summary.min = values[0];
            summary.max = values[c - 1];

            summary.median = values[c / 2];

            summary.lowerQ = values[c / 4];
            summary.upperQ = values[c / 4 * 3];

            return summary;
        }

        public float SD(float[] values)
        {
            int c = values.Length;
            if (c == 0)
            {
                return 0;
            }

            return SD(values, Mean(values));
        }
        public float SD(float[] values, float mean)
        {
            int c = values.Length;
            if (c == 0)
            {
                return 0;
            }

            // s = Sqrt(Sum((X - mean)^2)/(n-1))

            float sum, sd;

            sum = SumLessValueSquared(values, mean);
            sd = (float)Math.Sqrt(sum / (c - 1));

            return sd;
        }

        public float SE(float sd, float n)
        {
            return sd / (float)Math.Sqrt(n);
        }

        public MeanSummary StudentDist_CI(float[] values, float significance)
        {
            float mean, sd, se, n, df, t, lowerBound, upperBound;

            n = values.Length;
            mean = Mean(values);
            sd = SD(values, mean);
            se = SE(sd, n);
            df = n - 1;
            t = StudentDist_tValue(significance, df);

            //CI = mean +and- t * se
            lowerBound = mean - t * se;
            upperBound = mean + t * se;

            return new MeanSummary(lowerBound, mean, upperBound, se);
        }

        public float StudentDist_tValue(float significance, float df)
        {
            int sigIndex, dfIndex;
            float tValue;

            Closest(tValueSig, significance, out sigIndex);
            Closest(tValueDf, df, out dfIndex);

            tValue = tValues[sigIndex, dfIndex];

            return tValue;
        }

        public PointF NormDist_RandomPoint(PointF mean, double sd)
        {
            PointF p = new PointF(0, 0);

            p.X = NormDist_RandomX(mean.X, sd);
            p.Y = NormDist_RandomX(mean.Y, sd);

            return p;
        }

        public float NormDist_RandomX(double mean, double sd)
        {
            double rnProb;
            float x;

            rnProb = Random_NextDouble();

            x = NormDist_XfromProb(mean, sd, rnProb);

            return x;
        }

        public float NormDist_ProbOfX(double mean, double sd, double x)
        {
            if (sd == 0)
            {
                if (x == mean)
                {
                    return 1;
                }
                return 0;
            }
            else
            {
                float zV, prob;

                zV = (float)((x - mean) / sd);

                prob = NormDist_ProbFromZScore(zV);

                //Console.WriteLine("pr(X ~ Normal(" + mean + "," + sd + ") < " + x + ") = " + prob);

                return prob;
            }
        }

        public float NormDist_XfromProb(double mean, double sd, double prob)
        {
            if (sd == 0)
            {
                return (float)(mean);
            }
            else
            {
                float zV, x;

                zV = NormDist_ZScoreFromProb(prob);

                //z = (x - mean)/sd
                //x = z * sd + mean
                x = (float)(zV * sd + mean);

                //Console.WriteLine("pr(X ~ Normal(" + mean + "," + sd + ") < x) = " + prob + " X = " + x);

                return x;
            }
        }

        public float NormDist_ProbFromZScore(double ZScore)
        {
            //round z-score to 2 d.p.
            int zs;
            float prob;
            bool opposite = false;
            prob = 0;

            zs = (int)(Math.Round(ZScore, 2) * 100);

            if (zs < 0)
            {
                opposite = true;
                zs = -zs;
            }

            //max value
            if (zs >= z.Length - 1)
            {
                prob = 1;
            }
            else
            {
                prob = z[zs];
            }

            if (opposite)
            {
                prob = 1 - prob;
                zs = -zs;
            }

            //Console.WriteLine("ZScore: " + ZScore + ". Found zs: " + (float)(zs)/100 + ", prob: " + prob);

            return prob;
        }

        public float NormDist_ZScoreFromProb(double prob)
        {
            float dist, closestDist, zScore, fProb;
            bool opposite = false;
            closestDist = -1;
            zScore = 0;

            if (prob < 0.5)
            {
                opposite = true;
                prob = 1 - prob;
            }
            fProb = (float)(prob);

            //check critical values
            if (prob == 0.5)
            {
                zScore = 0;
            }
            //---------
            else if (prob == 0.75)
            {
                zScore = 0.674F;
            }
            else if (prob == 0.9)
            {
                zScore = 1.282F;
            }
            else if (prob == 0.95)
            {
                zScore = 1.645F;
            }
            //---------
            else if (prob == 0.975)
            {
                zScore = 1.960F;
            }
            else if (prob == 0.99)
            {
                zScore = 2.326F;
            }
            else if (prob == 0.995)
            {
                zScore = 2.576F;
            }
            //---------
            else if (prob == 0.9975)
            {
                zScore = 2.807F;
            }
            else if (prob == 0.999)
            {
                zScore = 3.090F;
            }
            else if (prob == 0.9995)
            {
                zScore = 3.291F;
            }
            else if (prob == 1)
            {
                zScore = 6.11F;
            }
            //---------
            else
            {
                //search z[] to find closest probability.
                //use difference in probability to find closest.

                for (int i = 0; i < z.Length; i++)
                {
                    dist = (float)(Math.Abs(prob - z[i]));
                    if (dist < closestDist || closestDist == -1)
                    {
                        closestDist = dist;
                        zScore = i;
                        fProb = z[i];
                    }
                }

                zScore /= 100;
            }

            if (opposite)
            {
                zScore = -zScore;
                prob = 1 - prob;
                fProb = 1 - fProb;
            }

            //Console.WriteLine("prob: " + prob + ". Found prob: " + fProb + ", zScore: " + zScore);

            return zScore;
        }

        public float Sum(float[] values)
        {
            int c = values.Length;
            if (c == 0)
            {
                return 0;
            }

            float total;

            total = 0;

            for (int i = 0; i < c; i++)
            {
                total += values[i];
            }

            return total;
        }
        public int Sum(int[] values)
        {
            int c = values.Length;
            if (c == 0)
            {
                return 0;
            }

            int total;

            total = 0;

            for (int i = 0; i < c; i++)
            {
                total += values[i];
            }

            return total;
        }

        //public void ComputeMinMaxInterval(Frequency frequencies, out float min, out float max, out float interval)
        //{

        //}

        public float SumSquared(float[] values)
        {
            int c = values.Length;
            if (c == 0)
            {
                return 0;
            }

            float total;

            total = 0;

            for (int i = 0; i < c; i++)
            {
                total += values[i] * values[i];
            }

            return total;
        }

        public float SumLessValueSquared(float[] values, float lessValue)
        {
            int c = values.Length;
            if (c == 0)
            {
                return 0;
            }

            float total, value;

            total = 0;

            for (int i = 0; i < c; i++)
            {
                value = values[i] - lessValue;
                total += value * value;
            }

            return total;
        }

        //public int FrequencyMax(float[,] frequencies)
        //{
        //    if (frequencies == null)
        //    {
        //        return 0;
        //    }

        //    int c = frequencies.GetLength(0);
        //    if (c == 0)
        //    {
        //        return 0;
        //    }

        //    int biggest = 0;

        //    for (int i = 0; i < c; i++)
        //    {
        //        if (frequencies[i, 1] > biggest)
        //        {
        //            biggest = (int)frequencies[i, 1];
        //        }
        //    }

        //    return biggest;
        //}

        //public float[] Sort(float[] values)
        //{
        //    Array.Sort(values);

        //    return Recreate(values);
        //}
        //public int[] Sort(int[] values)
        //{
        //    Array.Sort(values);

        //    return Recreate(values);
        //}
        //public DateTime[] Sort(DateTime[] values)
        //{
        //    Array.Sort(values);

        //    return Recreate(values);
        //}

        public float Mean(float[] values)
        {
            int c = values.Length;
            if (c == 0)
            {
                return 0;
            }

            float total, mean;

            total = Sum(values);

            mean = total / c;
            return mean;
        }

        public float Median(float[] values)
        {
            int c = values.Length;
            if (c == 0)
            {
                return 0;
            }

            values = Sort(values, true);

            float median;
            median = values[c / 2];

            return median;
        }

        public int Mode(Frequency frequencies)
        {
            int min, max;
            MinMaxValue(frequencies.frequencies, out min, out max);

            return max;
        }

        //-----------------------------------------------------------------------
        #endregion

        void Set_NormalDistributionValues()
        {

            // z*100
            z = new float[612];
            z[0] = 0.5F;
            z[1] = 0.503989356F;
            z[2] = 0.507978314F;
            z[3] = 0.511966473F;
            z[4] = 0.515953437F;
            z[5] = 0.519938806F;
            z[6] = 0.523922183F;
            z[7] = 0.52790317F;
            z[8] = 0.531881372F;
            z[9] = 0.535856393F;
            z[10] = 0.539827837F;
            z[11] = 0.543795313F;
            z[12] = 0.547758426F;
            z[13] = 0.551716787F;
            z[14] = 0.555670005F;
            z[15] = 0.559617692F;
            z[16] = 0.563559463F;
            z[17] = 0.567494932F;
            z[18] = 0.571423716F;
            z[19] = 0.575345435F;
            z[20] = 0.579259709F;
            z[21] = 0.583166163F;
            z[22] = 0.587064423F;
            z[23] = 0.590954115F;
            z[24] = 0.594834872F;
            z[25] = 0.598706326F;
            z[26] = 0.602568113F;
            z[27] = 0.606419873F;
            z[28] = 0.610261248F;
            z[29] = 0.614091881F;
            z[30] = 0.617911422F;
            z[31] = 0.621719522F;
            z[32] = 0.625515835F;
            z[33] = 0.629300019F;
            z[34] = 0.633071736F;
            z[35] = 0.636830651F;
            z[36] = 0.640576433F;
            z[37] = 0.644308755F;
            z[38] = 0.648027292F;
            z[39] = 0.651731727F;
            z[40] = 0.655421742F;
            z[41] = 0.659097026F;
            z[42] = 0.662757273F;
            z[43] = 0.666402179F;
            z[44] = 0.670031446F;
            z[45] = 0.67364478F;
            z[46] = 0.67724189F;
            z[47] = 0.680822491F;
            z[48] = 0.684386303F;
            z[49] = 0.687933051F;
            z[50] = 0.691462461F;
            z[51] = 0.694974269F;
            z[52] = 0.698468212F;
            z[53] = 0.701944035F;
            z[54] = 0.705401484F;
            z[55] = 0.708840313F;
            z[56] = 0.712260281F;
            z[57] = 0.715661151F;
            z[58] = 0.719042691F;
            z[59] = 0.722404675F;
            z[60] = 0.725746882F;
            z[61] = 0.729069096F;
            z[62] = 0.732371107F;
            z[63] = 0.735652708F;
            z[64] = 0.7389137F;
            z[65] = 0.742153889F;
            z[66] = 0.745373085F;
            z[67] = 0.748571105F;
            z[68] = 0.75174777F;
            z[69] = 0.754902906F;
            z[70] = 0.758036348F;
            z[71] = 0.761147932F;
            z[72] = 0.764237502F;
            z[73] = 0.767304908F;
            z[74] = 0.770350003F;
            z[75] = 0.773372648F;
            z[76] = 0.776372708F;
            z[77] = 0.779350054F;
            z[78] = 0.782304562F;
            z[79] = 0.785236116F;
            z[80] = 0.788144601F;
            z[81] = 0.791029912F;
            z[82] = 0.793891946F;
            z[83] = 0.796730608F;
            z[84] = 0.799545807F;
            z[85] = 0.802337457F;
            z[86] = 0.805105479F;
            z[87] = 0.807849798F;
            z[88] = 0.810570345F;
            z[89] = 0.813267057F;
            z[90] = 0.815939875F;
            z[91] = 0.818588745F;
            z[92] = 0.82121362F;
            z[93] = 0.823814458F;
            z[94] = 0.82639122F;
            z[95] = 0.828943874F;
            z[96] = 0.831472393F;
            z[97] = 0.833976754F;
            z[98] = 0.836456941F;
            z[99] = 0.83891294F;
            z[100] = 0.841344746F;
            z[101] = 0.843752355F;
            z[102] = 0.84613577F;
            z[103] = 0.848494997F;
            z[104] = 0.85083005F;
            z[105] = 0.853140944F;
            z[106] = 0.8554277F;
            z[107] = 0.857690346F;
            z[108] = 0.85992891F;
            z[109] = 0.862143428F;
            z[110] = 0.864333939F;
            z[111] = 0.866500487F;
            z[112] = 0.868643119F;
            z[113] = 0.870761888F;
            z[114] = 0.872856849F;
            z[115] = 0.874928064F;
            z[116] = 0.876975597F;
            z[117] = 0.878999516F;
            z[118] = 0.880999893F;
            z[119] = 0.882976804F;
            z[120] = 0.88493033F;
            z[121] = 0.886860554F;
            z[122] = 0.888767563F;
            z[123] = 0.890651448F;
            z[124] = 0.892512303F;
            z[125] = 0.894350226F;
            z[126] = 0.896165319F;
            z[127] = 0.897957685F;
            z[128] = 0.899727432F;
            z[129] = 0.901474671F;
            z[130] = 0.903199515F;
            z[131] = 0.904902082F;
            z[132] = 0.906582491F;
            z[133] = 0.908240864F;
            z[134] = 0.909877328F;
            z[135] = 0.911492009F;
            z[136] = 0.913085038F;
            z[137] = 0.914656549F;
            z[138] = 0.916206678F;
            z[139] = 0.917735561F;
            z[140] = 0.919243341F;
            z[141] = 0.920730159F;
            z[142] = 0.922196159F;
            z[143] = 0.92364149F;
            z[144] = 0.9250663F;
            z[145] = 0.92647074F;
            z[146] = 0.927854963F;
            z[147] = 0.929219123F;
            z[148] = 0.930563377F;
            z[149] = 0.931887882F;
            z[150] = 0.933192799F;
            z[151] = 0.934478288F;
            z[152] = 0.935744512F;
            z[153] = 0.936991636F;
            z[154] = 0.938219823F;
            z[155] = 0.939429242F;
            z[156] = 0.940620059F;
            z[157] = 0.941792444F;
            z[158] = 0.942946567F;
            z[159] = 0.944082597F;
            z[160] = 0.945200708F;
            z[161] = 0.946301072F;
            z[162] = 0.947383862F;
            z[163] = 0.948449252F;
            z[164] = 0.949497417F;
            z[165] = 0.950528532F;
            z[166] = 0.951542774F;
            z[167] = 0.952540318F;
            z[168] = 0.953521342F;
            z[169] = 0.954486023F;
            z[170] = 0.955434537F;
            z[171] = 0.956367063F;
            z[172] = 0.957283779F;
            z[173] = 0.958184862F;
            z[174] = 0.959070491F;
            z[175] = 0.959940843F;
            z[176] = 0.960796097F;
            z[177] = 0.96163643F;
            z[178] = 0.96246202F;
            z[179] = 0.963273044F;
            z[180] = 0.964069681F;
            z[181] = 0.964852106F;
            z[182] = 0.965620498F;
            z[183] = 0.966375031F;
            z[184] = 0.967115881F;
            z[185] = 0.967843225F;
            z[186] = 0.968557237F;
            z[187] = 0.969258091F;
            z[188] = 0.969945961F;
            z[189] = 0.97062102F;
            z[190] = 0.97128344F;
            z[191] = 0.971933393F;
            z[192] = 0.97257105F;
            z[193] = 0.973196581F;
            z[194] = 0.973810155F;
            z[195] = 0.97441194F;
            z[196] = 0.975002105F;
            z[197] = 0.975580815F;
            z[198] = 0.976148236F;
            z[199] = 0.976704532F;
            z[200] = 0.977249868F;
            z[201] = 0.977784406F;
            z[202] = 0.978308306F;
            z[203] = 0.97882173F;
            z[204] = 0.979324837F;
            z[205] = 0.979817785F;
            z[206] = 0.98030073F;
            z[207] = 0.980773828F;
            z[208] = 0.981237234F;
            z[209] = 0.9816911F;
            z[210] = 0.982135579F;
            z[211] = 0.982570822F;
            z[212] = 0.982996977F;
            z[213] = 0.983414193F;
            z[214] = 0.983822617F;
            z[215] = 0.984222393F;
            z[216] = 0.984613665F;
            z[217] = 0.984996577F;
            z[218] = 0.985371269F;
            z[219] = 0.985737882F;
            z[220] = 0.986096552F;
            z[221] = 0.986447419F;
            z[222] = 0.986790616F;
            z[223] = 0.987126279F;
            z[224] = 0.987454539F;
            z[225] = 0.987775527F;
            z[226] = 0.988089375F;
            z[227] = 0.988396208F;
            z[228] = 0.988696156F;
            z[229] = 0.988989342F;
            z[230] = 0.98927589F;
            z[231] = 0.989555923F;
            z[232] = 0.989829561F;
            z[233] = 0.990096924F;
            z[234] = 0.99035813F;
            z[235] = 0.990613294F;
            z[236] = 0.990862532F;
            z[237] = 0.991105957F;
            z[238] = 0.991343681F;
            z[239] = 0.991575814F;
            z[240] = 0.991802464F;
            z[241] = 0.99202374F;
            z[242] = 0.992239746F;
            z[243] = 0.992450589F;
            z[244] = 0.992656369F;
            z[245] = 0.992857189F;
            z[246] = 0.993053149F;
            z[247] = 0.993244347F;
            z[248] = 0.993430881F;
            z[249] = 0.993612845F;
            z[250] = 0.993790335F;
            z[251] = 0.993963442F;
            z[252] = 0.994132258F;
            z[253] = 0.994296874F;
            z[254] = 0.994457377F;
            z[255] = 0.994613854F;
            z[256] = 0.994766392F;
            z[257] = 0.994915074F;
            z[258] = 0.995059984F;
            z[259] = 0.995201203F;
            z[260] = 0.995338812F;
            z[261] = 0.995472889F;
            z[262] = 0.995603512F;
            z[263] = 0.995730757F;
            z[264] = 0.995854699F;
            z[265] = 0.995975411F;
            z[266] = 0.996092967F;
            z[267] = 0.996207438F;
            z[268] = 0.996318892F;
            z[269] = 0.996427399F;
            z[270] = 0.996533026F;
            z[271] = 0.99663584F;
            z[272] = 0.996735904F;
            z[273] = 0.996833284F;
            z[274] = 0.996928041F;
            z[275] = 0.997020237F;
            z[276] = 0.997109932F;
            z[277] = 0.997197185F;
            z[278] = 0.997282055F;
            z[279] = 0.997364598F;
            z[280] = 0.99744487F;
            z[281] = 0.997522925F;
            z[282] = 0.997598818F;
            z[283] = 0.9976726F;
            z[284] = 0.997744323F;
            z[285] = 0.997814039F;
            z[286] = 0.997881795F;
            z[287] = 0.997947641F;
            z[288] = 0.998011624F;
            z[289] = 0.998073791F;
            z[290] = 0.998134187F;
            z[291] = 0.998192856F;
            z[292] = 0.998249843F;
            z[293] = 0.99830519F;
            z[294] = 0.998358939F;
            z[295] = 0.99841113F;
            z[296] = 0.998461805F;
            z[297] = 0.998511001F;
            z[298] = 0.998558758F;
            z[299] = 0.998605113F;
            z[300] = 0.998650102F;
            z[301] = 0.998693762F;
            z[302] = 0.998736127F;
            z[303] = 0.998777231F;
            z[304] = 0.998817109F;
            z[305] = 0.998855793F;
            z[306] = 0.998893315F;
            z[307] = 0.998929706F;
            z[308] = 0.998964997F;
            z[309] = 0.998999218F;
            z[310] = 0.999032397F;
            z[311] = 0.999064563F;
            z[312] = 0.999095745F;
            z[313] = 0.999125968F;
            z[314] = 0.999155261F;
            z[315] = 0.999183648F;
            z[316] = 0.999211154F;
            z[317] = 0.999237805F;
            z[318] = 0.999263625F;
            z[319] = 0.999288636F;
            z[320] = 0.999312862F;
            z[321] = 0.999336325F;
            z[322] = 0.999359047F;
            z[323] = 0.999381049F;
            z[324] = 0.999402352F;
            z[325] = 0.999422975F;
            z[326] = 0.999442939F;
            z[327] = 0.999462263F;
            z[328] = 0.999480965F;
            z[329] = 0.999499063F;
            z[330] = 0.999516576F;
            z[331] = 0.99953352F;
            z[332] = 0.999549913F;
            z[333] = 0.99956577F;
            z[334] = 0.999581108F;
            z[335] = 0.999595942F;
            z[336] = 0.999610288F;
            z[337] = 0.999624159F;
            z[338] = 0.999637571F;
            z[339] = 0.999650537F;
            z[340] = 0.999663071F;
            z[341] = 0.999675186F;
            z[342] = 0.999686894F;
            z[343] = 0.999698209F;
            z[344] = 0.999709143F;
            z[345] = 0.999719707F;
            z[346] = 0.999729912F;
            z[347] = 0.999739771F;
            z[348] = 0.999749293F;
            z[349] = 0.99975849F;
            z[350] = 0.999767371F;
            z[351] = 0.999775947F;
            z[352] = 0.999784227F;
            z[353] = 0.99979222F;
            z[354] = 0.999799936F;
            z[355] = 0.999807384F;
            z[356] = 0.999814573F;
            z[357] = 0.999821509F;
            z[358] = 0.999828203F;
            z[359] = 0.999834661F;
            z[360] = 0.999840891F;
            z[361] = 0.999846901F;
            z[362] = 0.999852698F;
            z[363] = 0.999858289F;
            z[364] = 0.999863681F;
            z[365] = 0.99986888F;
            z[366] = 0.999873892F;
            z[367] = 0.999878725F;
            z[368] = 0.999883383F;
            z[369] = 0.999887873F;
            z[370] = 0.9998922F;
            z[371] = 0.99989637F;
            z[372] = 0.999900389F;
            z[373] = 0.99990426F;
            z[374] = 0.99990799F;
            z[375] = 0.999911583F;
            z[376] = 0.999915043F;
            z[377] = 0.999918376F;
            z[378] = 0.999921586F;
            z[379] = 0.999924676F;
            z[380] = 0.999927652F;
            z[381] = 0.999930517F;
            z[382] = 0.999933274F;
            z[383] = 0.999935928F;
            z[384] = 0.999938483F;
            z[385] = 0.999940941F;
            z[386] = 0.999943306F;
            z[387] = 0.999945582F;
            z[388] = 0.999947772F;
            z[389] = 0.999949878F;
            z[390] = 0.999951904F;
            z[391] = 0.999953852F;
            z[392] = 0.999955726F;
            z[393] = 0.999957527F;
            z[394] = 0.999959259F;
            z[395] = 0.999960924F;
            z[396] = 0.999962525F;
            z[397] = 0.999964064F;
            z[398] = 0.999965542F;
            z[399] = 0.999966963F;
            z[400] = 0.999968329F;
            z[401] = 0.999969641F;
            z[402] = 0.999970901F;
            z[403] = 0.999972112F;
            z[404] = 0.999973274F;
            z[405] = 0.999974391F;
            z[406] = 0.999975464F;
            z[407] = 0.999976493F;
            z[408] = 0.999977482F;
            z[409] = 0.999978431F;
            z[410] = 0.999979342F;
            z[411] = 0.999980217F;
            z[412] = 0.999981056F;
            z[413] = 0.999981862F;
            z[414] = 0.999982635F;
            z[415] = 0.999983376F;
            z[416] = 0.999984088F;
            z[417] = 0.99998477F;
            z[418] = 0.999985425F;
            z[419] = 0.999986052F;
            z[420] = 0.999986654F;
            z[421] = 0.999987231F;
            z[422] = 0.999987785F;
            z[423] = 0.999988315F;
            z[424] = 0.999988824F;
            z[425] = 0.999989311F;
            z[426] = 0.999989779F;
            z[427] = 0.999990226F;
            z[428] = 0.999990655F;
            z[429] = 0.999991066F;
            z[430] = 0.99999146F;
            z[431] = 0.999991837F;
            z[432] = 0.999992199F;
            z[433] = 0.999992545F;
            z[434] = 0.999992876F;
            z[435] = 0.999993193F;
            z[436] = 0.999993497F;
            z[437] = 0.999993788F;
            z[438] = 0.999994066F;
            z[439] = 0.999994332F;
            z[440] = 0.999994587F;
            z[441] = 0.999994831F;
            z[442] = 0.999995065F;
            z[443] = 0.999995288F;
            z[444] = 0.999995502F;
            z[445] = 0.999995706F;
            z[446] = 0.999995902F;
            z[447] = 0.999996089F;
            z[448] = 0.999996268F;
            z[449] = 0.999996439F;
            z[450] = 0.999996602F;
            z[451] = 0.999996759F;
            z[452] = 0.999996908F;
            z[453] = 0.999997051F;
            z[454] = 0.999997187F;
            z[455] = 0.999997318F;
            z[456] = 0.999997442F;
            z[457] = 0.999997561F;
            z[458] = 0.999997675F;
            z[459] = 0.999997784F;
            z[460] = 0.999997888F;
            z[461] = 0.999997987F;
            z[462] = 0.999998081F;
            z[463] = 0.999998172F;
            z[464] = 0.999998258F;
            z[465] = 0.99999834F;
            z[466] = 0.999998419F;
            z[467] = 0.999998494F;
            z[468] = 0.999998566F;
            z[469] = 0.999998634F;
            z[470] = 0.999998699F;
            z[471] = 0.999998761F;
            z[472] = 0.999998821F;
            z[473] = 0.999998877F;
            z[474] = 0.999998931F;
            z[475] = 0.999998983F;
            z[476] = 0.999999032F;
            z[477] = 0.999999079F;
            z[478] = 0.999999124F;
            z[479] = 0.999999166F;
            z[480] = 0.999999207F;
            z[481] = 0.999999245F;
            z[482] = 0.999999282F;
            z[483] = 0.999999317F;
            z[484] = 0.999999351F;
            z[485] = 0.999999383F;
            z[486] = 0.999999413F;
            z[487] = 0.999999442F;
            z[488] = 0.99999947F;
            z[489] = 0.999999496F;
            z[490] = 0.999999521F;
            z[491] = 0.999999545F;
            z[492] = 0.999999567F;
            z[493] = 0.999999589F;
            z[494] = 0.999999609F;
            z[495] = 0.999999629F;
            z[496] = 0.999999648F;
            z[497] = 0.999999665F;
            z[498] = 0.999999682F;
            z[499] = 0.999999698F;
            z[500] = 0.999999713F;
            z[501] = 0.999999728F;
            z[502] = 0.999999742F;
            z[503] = 0.999999755F;
            z[504] = 0.999999767F;
            z[505] = 0.999999779F;
            z[506] = 0.99999979F;
            z[507] = 0.999999801F;
            z[508] = 0.999999811F;
            z[509] = 0.999999821F;
            z[510] = 0.99999983F;
            z[511] = 0.999999839F;
            z[512] = 0.999999847F;
            z[513] = 0.999999855F;
            z[514] = 0.999999863F;
            z[515] = 0.99999987F;
            z[516] = 0.999999877F;
            z[517] = 0.999999883F;
            z[518] = 0.999999889F;
            z[519] = 0.999999895F;
            z[520] = 0.9999999F;
            z[521] = 0.999999906F;
            z[522] = 0.999999911F;
            z[523] = 0.999999915F;
            z[524] = 0.99999992F;
            z[525] = 0.999999924F;
            z[526] = 0.999999928F;
            z[527] = 0.999999932F;
            z[528] = 0.999999935F;
            z[529] = 0.999999939F;
            z[530] = 0.999999942F;
            z[531] = 0.999999945F;
            z[532] = 0.999999948F;
            z[533] = 0.999999951F;
            z[534] = 0.999999954F;
            z[535] = 0.999999956F;
            z[536] = 0.999999958F;
            z[537] = 0.999999961F;
            z[538] = 0.999999963F;
            z[539] = 0.999999965F;
            z[540] = 0.999999967F;
            z[541] = 0.999999968F;
            z[542] = 0.99999997F;
            z[543] = 0.999999972F;
            z[544] = 0.999999973F;
            z[545] = 0.999999975F;
            z[546] = 0.999999976F;
            z[547] = 0.999999977F;
            z[548] = 0.999999979F;
            z[549] = 0.99999998F;
            z[550] = 0.999999981F;
            z[551] = 0.999999982F;
            z[552] = 0.999999983F;
            z[553] = 0.999999984F;
            z[554] = 0.999999985F;
            z[555] = 0.999999986F;
            z[556] = 0.999999987F;
            z[557] = 0.999999987F;
            z[558] = 0.999999988F;
            z[559] = 0.999999989F;
            z[560] = 0.999999989F;
            z[561] = 0.99999999F;
            z[562] = 0.99999999F;
            z[563] = 0.999999991F;
            z[564] = 0.999999991F;
            z[565] = 0.999999992F;
            z[566] = 0.999999992F;
            z[567] = 0.999999993F;
            z[568] = 0.999999993F;
            z[569] = 0.999999994F;
            z[570] = 0.999999994F;
            z[571] = 0.999999994F;
            z[572] = 0.999999995F;
            z[573] = 0.999999995F;
            z[574] = 0.999999995F;
            z[575] = 0.999999996F;
            z[576] = 0.999999996F;
            z[577] = 0.999999996F;
            z[578] = 0.999999996F;
            z[579] = 0.999999996F;
            z[580] = 0.999999997F;
            z[581] = 0.999999997F;
            z[582] = 0.999999997F;
            z[583] = 0.999999997F;
            z[584] = 0.999999997F;
            z[585] = 0.999999998F;
            z[586] = 0.999999998F;
            z[587] = 0.999999998F;
            z[588] = 0.999999998F;
            z[589] = 0.999999998F;
            z[590] = 0.999999998F;
            z[591] = 0.999999998F;
            z[592] = 0.999999998F;
            z[593] = 0.999999998F;
            z[594] = 0.999999999F;
            z[595] = 0.999999999F;
            z[596] = 0.999999999F;
            z[597] = 0.999999999F;
            z[598] = 0.999999999F;
            z[599] = 0.999999999F;
            z[600] = 0.999999999F;
            z[601] = 0.999999999F;
            z[602] = 0.999999999F;
            z[603] = 0.999999999F;
            z[604] = 0.999999999F;
            z[605] = 0.999999999F;
            z[606] = 0.999999999F;
            z[607] = 0.999999999F;
            z[608] = 0.999999999F;
            z[609] = 0.999999999F;
            z[610] = 0.999999999F;
            z[611] = 1F;
        }

        void Set_tValues()
        {
            tValueSig = new float[11] { 25.00f, 20.00f, 15.00f, 10.00f, 5.00f, 2.50f, 1.00f, 0.50f, 0.25f, 0.10f, 0.05f };

            tValueDf = new float[37]
            {
                1f,2f,3f,4f,5f,6f,7f,8f,9f,10f,11f,12f,13f,14f,15f,16f,17f,18f,19f,20f,21f,22f,23f,24f,25f,26f,27f,28f,29f,30f,40f,50f,60f,80f,100f,120f,float.PositiveInfinity
            };

            tValues = new float[11, 37];

            //75%
            tValues[0, 0] = 1f;
            tValues[0, 1] = 0.816f;
            tValues[0, 2] = 0.765f;
            tValues[0, 3] = 0.741f;
            tValues[0, 4] = 0.727f;
            tValues[0, 5] = 0.718f;
            tValues[0, 6] = 0.711f;
            tValues[0, 7] = 0.706f;
            tValues[0, 8] = 0.703f;
            tValues[0, 9] = 0.7f;
            tValues[0, 10] = 0.697f;
            tValues[0, 11] = 0.695f;
            tValues[0, 12] = 0.694f;
            tValues[0, 13] = 0.692f;
            tValues[0, 14] = 0.691f;
            tValues[0, 15] = 0.69f;
            tValues[0, 16] = 0.689f;
            tValues[0, 17] = 0.688f;
            tValues[0, 18] = 0.688f;
            tValues[0, 19] = 0.687f;
            tValues[0, 20] = 0.686f;
            tValues[0, 21] = 0.686f;
            tValues[0, 22] = 0.685f;
            tValues[0, 23] = 0.685f;
            tValues[0, 24] = 0.684f;
            tValues[0, 25] = 0.684f;
            tValues[0, 26] = 0.684f;
            tValues[0, 27] = 0.683f;
            tValues[0, 28] = 0.683f;
            tValues[0, 29] = 0.683f;
            tValues[0, 30] = 0.681f;
            tValues[0, 31] = 0.679f;
            tValues[0, 32] = 0.679f;
            tValues[0, 33] = 0.678f;
            tValues[0, 34] = 0.677f;
            tValues[0, 35] = 0.677f;
            tValues[0, 36] = 0.674f;
            //80%
            tValues[1, 0] = 1.376f;
            tValues[1, 1] = 1.061f;
            tValues[1, 2] = 0.978f;
            tValues[1, 3] = 0.941f;
            tValues[1, 4] = 0.92f;
            tValues[1, 5] = 0.906f;
            tValues[1, 6] = 0.896f;
            tValues[1, 7] = 0.889f;
            tValues[1, 8] = 0.883f;
            tValues[1, 9] = 0.879f;
            tValues[1, 10] = 0.876f;
            tValues[1, 11] = 0.873f;
            tValues[1, 12] = 0.87f;
            tValues[1, 13] = 0.868f;
            tValues[1, 14] = 0.866f;
            tValues[1, 15] = 0.865f;
            tValues[1, 16] = 0.863f;
            tValues[1, 17] = 0.862f;
            tValues[1, 18] = 0.861f;
            tValues[1, 19] = 0.86f;
            tValues[1, 20] = 0.859f;
            tValues[1, 21] = 0.858f;
            tValues[1, 22] = 0.858f;
            tValues[1, 23] = 0.857f;
            tValues[1, 24] = 0.856f;
            tValues[1, 25] = 0.856f;
            tValues[1, 26] = 0.855f;
            tValues[1, 27] = 0.855f;
            tValues[1, 28] = 0.854f;
            tValues[1, 29] = 0.854f;
            tValues[1, 30] = 0.851f;
            tValues[1, 31] = 0.849f;
            tValues[1, 32] = 0.848f;
            tValues[1, 33] = 0.846f;
            tValues[1, 34] = 0.845f;
            tValues[1, 35] = 0.845f;
            tValues[1, 36] = 0.842f;
            //85%
            tValues[2, 0] = 1.963f;
            tValues[2, 1] = 1.386f;
            tValues[2, 2] = 1.25f;
            tValues[2, 3] = 1.19f;
            tValues[2, 4] = 1.156f;
            tValues[2, 5] = 1.134f;
            tValues[2, 6] = 1.119f;
            tValues[2, 7] = 1.108f;
            tValues[2, 8] = 1.1f;
            tValues[2, 9] = 1.093f;
            tValues[2, 10] = 1.088f;
            tValues[2, 11] = 1.083f;
            tValues[2, 12] = 1.079f;
            tValues[2, 13] = 1.076f;
            tValues[2, 14] = 1.074f;
            tValues[2, 15] = 1.071f;
            tValues[2, 16] = 1.069f;
            tValues[2, 17] = 1.067f;
            tValues[2, 18] = 1.066f;
            tValues[2, 19] = 1.064f;
            tValues[2, 20] = 1.063f;
            tValues[2, 21] = 1.061f;
            tValues[2, 22] = 1.06f;
            tValues[2, 23] = 1.059f;
            tValues[2, 24] = 1.058f;
            tValues[2, 25] = 1.058f;
            tValues[2, 26] = 1.057f;
            tValues[2, 27] = 1.056f;
            tValues[2, 28] = 1.055f;
            tValues[2, 29] = 1.055f;
            tValues[2, 30] = 1.05f;
            tValues[2, 31] = 1.047f;
            tValues[2, 32] = 1.045f;
            tValues[2, 33] = 1.043f;
            tValues[2, 34] = 1.042f;
            tValues[2, 35] = 1.041f;
            tValues[2, 36] = 1.036f;
            //90%
            tValues[3, 0] = 3.078f;
            tValues[3, 1] = 1.886f;
            tValues[3, 2] = 1.638f;
            tValues[3, 3] = 1.533f;
            tValues[3, 4] = 1.476f;
            tValues[3, 5] = 1.44f;
            tValues[3, 6] = 1.415f;
            tValues[3, 7] = 1.397f;
            tValues[3, 8] = 1.383f;
            tValues[3, 9] = 1.372f;
            tValues[3, 10] = 1.363f;
            tValues[3, 11] = 1.356f;
            tValues[3, 12] = 1.35f;
            tValues[3, 13] = 1.345f;
            tValues[3, 14] = 1.341f;
            tValues[3, 15] = 1.337f;
            tValues[3, 16] = 1.333f;
            tValues[3, 17] = 1.33f;
            tValues[3, 18] = 1.328f;
            tValues[3, 19] = 1.325f;
            tValues[3, 20] = 1.323f;
            tValues[3, 21] = 1.321f;
            tValues[3, 22] = 1.319f;
            tValues[3, 23] = 1.318f;
            tValues[3, 24] = 1.316f;
            tValues[3, 25] = 1.315f;
            tValues[3, 26] = 1.314f;
            tValues[3, 27] = 1.313f;
            tValues[3, 28] = 1.311f;
            tValues[3, 29] = 1.31f;
            tValues[3, 30] = 1.303f;
            tValues[3, 31] = 1.299f;
            tValues[3, 32] = 1.296f;
            tValues[3, 33] = 1.292f;
            tValues[3, 34] = 1.29f;
            tValues[3, 35] = 1.289f;
            tValues[3, 36] = 1.282f;
            //95%
            tValues[4, 0] = 6.314f;
            tValues[4, 1] = 2.92f;
            tValues[4, 2] = 2.353f;
            tValues[4, 3] = 2.132f;
            tValues[4, 4] = 2.015f;
            tValues[4, 5] = 1.943f;
            tValues[4, 6] = 1.895f;
            tValues[4, 7] = 1.86f;
            tValues[4, 8] = 1.833f;
            tValues[4, 9] = 1.812f;
            tValues[4, 10] = 1.796f;
            tValues[4, 11] = 1.782f;
            tValues[4, 12] = 1.771f;
            tValues[4, 13] = 1.761f;
            tValues[4, 14] = 1.753f;
            tValues[4, 15] = 1.746f;
            tValues[4, 16] = 1.74f;
            tValues[4, 17] = 1.734f;
            tValues[4, 18] = 1.729f;
            tValues[4, 19] = 1.725f;
            tValues[4, 20] = 1.721f;
            tValues[4, 21] = 1.717f;
            tValues[4, 22] = 1.714f;
            tValues[4, 23] = 1.711f;
            tValues[4, 24] = 1.708f;
            tValues[4, 25] = 1.706f;
            tValues[4, 26] = 1.703f;
            tValues[4, 27] = 1.701f;
            tValues[4, 28] = 1.699f;
            tValues[4, 29] = 1.697f;
            tValues[4, 30] = 1.684f;
            tValues[4, 31] = 1.676f;
            tValues[4, 32] = 1.671f;
            tValues[4, 33] = 1.664f;
            tValues[4, 34] = 1.66f;
            tValues[4, 35] = 1.658f;
            tValues[4, 36] = 1.645f;
            //97.5%
            tValues[5, 0] = 12.71f;
            tValues[5, 1] = 4.303f;
            tValues[5, 2] = 3.182f;
            tValues[5, 3] = 2.776f;
            tValues[5, 4] = 2.571f;
            tValues[5, 5] = 2.447f;
            tValues[5, 6] = 2.365f;
            tValues[5, 7] = 2.306f;
            tValues[5, 8] = 2.262f;
            tValues[5, 9] = 2.228f;
            tValues[5, 10] = 2.201f;
            tValues[5, 11] = 2.179f;
            tValues[5, 12] = 2.16f;
            tValues[5, 13] = 2.145f;
            tValues[5, 14] = 2.131f;
            tValues[5, 15] = 2.12f;
            tValues[5, 16] = 2.11f;
            tValues[5, 17] = 2.101f;
            tValues[5, 18] = 2.093f;
            tValues[5, 19] = 2.086f;
            tValues[5, 20] = 2.08f;
            tValues[5, 21] = 2.074f;
            tValues[5, 22] = 2.069f;
            tValues[5, 23] = 2.064f;
            tValues[5, 24] = 2.06f;
            tValues[5, 25] = 2.056f;
            tValues[5, 26] = 2.052f;
            tValues[5, 27] = 2.048f;
            tValues[5, 28] = 2.045f;
            tValues[5, 29] = 2.042f;
            tValues[5, 30] = 2.021f;
            tValues[5, 31] = 2.009f;
            tValues[5, 32] = 2f;
            tValues[5, 33] = 1.99f;
            tValues[5, 34] = 1.984f;
            tValues[5, 35] = 1.98f;
            tValues[5, 36] = 1.96f;
            //99%
            tValues[6, 0] = 31.82f;
            tValues[6, 1] = 6.965f;
            tValues[6, 2] = 4.541f;
            tValues[6, 3] = 3.747f;
            tValues[6, 4] = 3.365f;
            tValues[6, 5] = 3.143f;
            tValues[6, 6] = 2.998f;
            tValues[6, 7] = 2.896f;
            tValues[6, 8] = 2.821f;
            tValues[6, 9] = 2.764f;
            tValues[6, 10] = 2.718f;
            tValues[6, 11] = 2.681f;
            tValues[6, 12] = 2.65f;
            tValues[6, 13] = 2.624f;
            tValues[6, 14] = 2.602f;
            tValues[6, 15] = 2.583f;
            tValues[6, 16] = 2.567f;
            tValues[6, 17] = 2.552f;
            tValues[6, 18] = 2.539f;
            tValues[6, 19] = 2.528f;
            tValues[6, 20] = 2.518f;
            tValues[6, 21] = 2.508f;
            tValues[6, 22] = 2.5f;
            tValues[6, 23] = 2.492f;
            tValues[6, 24] = 2.485f;
            tValues[6, 25] = 2.479f;
            tValues[6, 26] = 2.473f;
            tValues[6, 27] = 2.467f;
            tValues[6, 28] = 2.462f;
            tValues[6, 29] = 2.457f;
            tValues[6, 30] = 2.423f;
            tValues[6, 31] = 2.403f;
            tValues[6, 32] = 2.39f;
            tValues[6, 33] = 2.374f;
            tValues[6, 34] = 2.364f;
            tValues[6, 35] = 2.358f;
            tValues[6, 36] = 2.326f;
            //99.5%
            tValues[7, 0] = 63.66f;
            tValues[7, 1] = 9.925f;
            tValues[7, 2] = 5.841f;
            tValues[7, 3] = 4.604f;
            tValues[7, 4] = 4.032f;
            tValues[7, 5] = 3.707f;
            tValues[7, 6] = 3.499f;
            tValues[7, 7] = 3.355f;
            tValues[7, 8] = 3.25f;
            tValues[7, 9] = 3.169f;
            tValues[7, 10] = 3.106f;
            tValues[7, 11] = 3.055f;
            tValues[7, 12] = 3.012f;
            tValues[7, 13] = 2.977f;
            tValues[7, 14] = 2.947f;
            tValues[7, 15] = 2.921f;
            tValues[7, 16] = 2.898f;
            tValues[7, 17] = 2.878f;
            tValues[7, 18] = 2.861f;
            tValues[7, 19] = 2.845f;
            tValues[7, 20] = 2.831f;
            tValues[7, 21] = 2.819f;
            tValues[7, 22] = 2.807f;
            tValues[7, 23] = 2.797f;
            tValues[7, 24] = 2.787f;
            tValues[7, 25] = 2.779f;
            tValues[7, 26] = 2.771f;
            tValues[7, 27] = 2.763f;
            tValues[7, 28] = 2.756f;
            tValues[7, 29] = 2.75f;
            tValues[7, 30] = 2.704f;
            tValues[7, 31] = 2.678f;
            tValues[7, 32] = 2.66f;
            tValues[7, 33] = 2.639f;
            tValues[7, 34] = 2.626f;
            tValues[7, 35] = 2.617f;
            tValues[7, 36] = 2.576f;
            //99.75%
            tValues[8, 0] = 127.3f;
            tValues[8, 1] = 14.09f;
            tValues[8, 2] = 7.453f;
            tValues[8, 3] = 5.598f;
            tValues[8, 4] = 4.773f;
            tValues[8, 5] = 4.317f;
            tValues[8, 6] = 4.029f;
            tValues[8, 7] = 3.833f;
            tValues[8, 8] = 3.69f;
            tValues[8, 9] = 3.581f;
            tValues[8, 10] = 3.497f;
            tValues[8, 11] = 3.428f;
            tValues[8, 12] = 3.372f;
            tValues[8, 13] = 3.326f;
            tValues[8, 14] = 3.286f;
            tValues[8, 15] = 3.252f;
            tValues[8, 16] = 3.222f;
            tValues[8, 17] = 3.197f;
            tValues[8, 18] = 3.174f;
            tValues[8, 19] = 3.153f;
            tValues[8, 20] = 3.135f;
            tValues[8, 21] = 3.119f;
            tValues[8, 22] = 3.104f;
            tValues[8, 23] = 3.091f;
            tValues[8, 24] = 3.078f;
            tValues[8, 25] = 3.067f;
            tValues[8, 26] = 3.057f;
            tValues[8, 27] = 3.047f;
            tValues[8, 28] = 3.038f;
            tValues[8, 29] = 3.03f;
            tValues[8, 30] = 2.971f;
            tValues[8, 31] = 2.937f;
            tValues[8, 32] = 2.915f;
            tValues[8, 33] = 2.887f;
            tValues[8, 34] = 2.871f;
            tValues[8, 35] = 2.86f;
            tValues[8, 36] = 2.807f;
            //99.9%
            tValues[9, 0] = 318.3f;
            tValues[9, 1] = 22.33f;
            tValues[9, 2] = 10.21f;
            tValues[9, 3] = 7.173f;
            tValues[9, 4] = 5.893f;
            tValues[9, 5] = 5.208f;
            tValues[9, 6] = 4.785f;
            tValues[9, 7] = 4.501f;
            tValues[9, 8] = 4.297f;
            tValues[9, 9] = 4.144f;
            tValues[9, 10] = 4.025f;
            tValues[9, 11] = 3.93f;
            tValues[9, 12] = 3.852f;
            tValues[9, 13] = 3.787f;
            tValues[9, 14] = 3.733f;
            tValues[9, 15] = 3.686f;
            tValues[9, 16] = 3.646f;
            tValues[9, 17] = 3.61f;
            tValues[9, 18] = 3.579f;
            tValues[9, 19] = 3.552f;
            tValues[9, 20] = 3.527f;
            tValues[9, 21] = 3.505f;
            tValues[9, 22] = 3.485f;
            tValues[9, 23] = 3.467f;
            tValues[9, 24] = 3.45f;
            tValues[9, 25] = 3.435f;
            tValues[9, 26] = 3.421f;
            tValues[9, 27] = 3.408f;
            tValues[9, 28] = 3.396f;
            tValues[9, 29] = 3.385f;
            tValues[9, 30] = 3.307f;
            tValues[9, 31] = 3.261f;
            tValues[9, 32] = 3.232f;
            tValues[9, 33] = 3.195f;
            tValues[9, 34] = 3.174f;
            tValues[9, 35] = 3.16f;
            tValues[9, 36] = 3.09f;
            //99.95%
            tValues[10, 0] = 636.6f;
            tValues[10, 1] = 31.6f;
            tValues[10, 2] = 12.92f;
            tValues[10, 3] = 8.61f;
            tValues[10, 4] = 6.869f;
            tValues[10, 5] = 5.959f;
            tValues[10, 6] = 5.408f;
            tValues[10, 7] = 5.041f;
            tValues[10, 8] = 4.781f;
            tValues[10, 9] = 4.587f;
            tValues[10, 10] = 4.437f;
            tValues[10, 11] = 4.318f;
            tValues[10, 12] = 4.221f;
            tValues[10, 13] = 4.14f;
            tValues[10, 14] = 4.073f;
            tValues[10, 15] = 4.015f;
            tValues[10, 16] = 3.965f;
            tValues[10, 17] = 3.922f;
            tValues[10, 18] = 3.883f;
            tValues[10, 19] = 3.85f;
            tValues[10, 20] = 3.819f;
            tValues[10, 21] = 3.792f;
            tValues[10, 22] = 3.767f;
            tValues[10, 23] = 3.745f;
            tValues[10, 24] = 3.725f;
            tValues[10, 25] = 3.707f;
            tValues[10, 26] = 3.69f;
            tValues[10, 27] = 3.674f;
            tValues[10, 28] = 3.659f;
            tValues[10, 29] = 3.646f;
            tValues[10, 30] = 3.551f;
            tValues[10, 31] = 3.496f;
            tValues[10, 32] = 3.46f;
            tValues[10, 33] = 3.416f;
            tValues[10, 34] = 3.39f;
            tValues[10, 35] = 3.373f;
            tValues[10, 36] = 3.291f;


        }

        //--------------

        public Point ConvertTo(PointF p)
        {
            return new Point((int)p.X, (int)p.Y);
        }

        public Point Ceiling(PointF p)
        {
            return new Point((int)Math.Ceiling(p.X), (int)Math.Ceiling(p.Y));
        }

        public Point[] Clean(Point[] ps)
        {
            int c, count, index;

            count = 0;
            index = 0;

            c = ps.Length;
            for (int i = 0; i < c; i++)
            {
                if (ps[i] != Point.Empty)
                {
                    count++;
                }
            }

            Point[] ps2 = new Point[count];

            for (int i = 0; i < c; i++)
            {
                if (ps[i] != Point.Empty)
                {
                    ps2[index] = ps[i];
                    index++;
                }
                else
                {

                }
            }

            return ps2;
        }

        public long SecondsToTicks(double seconds)
        {
            return (long)(seconds * 10000000.0);
        }

        public string ToString(TimeSpan time)
        {
            return time.Days + ":" + string.Format("{0:00}", time.Hours) + ":" + string.Format("{0:00}", time.Minutes) + ":" + string.Format("{0:00}", time.Seconds);
        }

        public float[] Mean_Row(float[,] values)
        {
            int c, cc;
            float[] ave;
            float total;

            c = values.GetLength(0);
            cc = values.GetLength(1);

            ave = new float[c];

            for (int i = 0; i < c; i++) //row
            {
                total = 0;
                for (int v = 0; v < cc; v++) //column
                {
                    total += values[i, v];
                }
                ave[i] = total / cc;
            }

            return ave;
        }

        public float[,] UpdateRows(float[,] values, float[] newColumn)
        {
            //move all columns -1
            float[,] newValues;
            int columns, rows;

            newValues = new float[values.GetLength(0), values.GetLength(1)];

            columns = values.GetLength(0);
            rows = values.GetLength(1);

            for (int row = 0; row < rows - 1; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    newValues[column, row + 1] = values[column, row];
                }
            }

            //insert newValues into first column
            for (int column = 0; column < columns; column++) //row
            {
                newValues[column, 0] = newColumn[column];
            }

            return newValues;
        }

        public PointF[] TangentsToCircleFromPoint(PointF p, PointF circleP, float radius)
        {
            PointF[] ps = new PointF[2];
            PointF p1;
            double hyp, adj, angle;

            hyp = Distance(p, circleP);
            //sin() = opp/hyp
            angle = radius / hyp;

            //cos() = adj/hyp
            adj = hyp * Math.Cos(angle);

            //right
            p1 = CircularMovement_Angle(new PointF(), Dir(p, circleP), angle, true);
            p1 = Offset(p1, (float)adj);
            ps[0] = Add(p1, p);

            //left
            p1 = CircularMovement_Angle(new PointF(), Dir(p, circleP), angle, false);
            p1 = Offset(p1, (float)adj);
            ps[1] = Add(p1, p);

            return ps;
        }

        public double Random_NextDouble()
        {
            Random rn = new Random();
            return rn.NextDouble();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lowerBound">inclusive</param>
        /// <param name="upperBound">exclusive</param>
        /// <returns></returns>
        public int Random_Next(int lowerBound, int upperBound)
        {
            Random rn = new Random();
            return rn.Next(lowerBound, upperBound);
        }
    }

    class v3
    {
        public double X, Y, Z;

        public v3(double _X, double _Y, double _Z)
        {
            X = _X;
            Y = _Y;
            Z = _Z;
        }
        public v3()
        {
        }

        public override string ToString()
        {
            return "(" + X + "," + Y + "," + Z + ")";
        }
        public string ToString(bool unitary)
        {
            if (unitary)
            {
                double m;
                m = System.Math.Sqrt(X * X + Y * Y + Z * Z);

                return "(" + (X / m) + "," + (Y / m) + "," + (Z / m) + ")";
            }
            else
            {
                return "(" + X + "," + Y + "," + Z + ")";
            }
        }
    }

    class v2
    {
        public double X, Y;

        public v2(double _X, double _Y)
        {
            X = _X;
            Y = _Y;
        }
        public v2()
        {

        }

        public override string ToString()
        {
            return "(" + X + "," + Y + ")";
        }
    }

    class Frequency
    {
        public float[] values;
        public int[] frequencies;
        public float interval;

        public Frequency(float[] _values, int[] _frequencies, float _interval)
        {
            values = _values;
            frequencies = _frequencies;
            interval = _interval;
        }
        public Frequency() { }

        public override string ToString()
        {
            int c = frequencies.GetLength(0);
            if (c == 0)
            {
                return "";
            }
            string str = "";

            for (int i = 0; i < c; i++)
            {
                if (i != 0)
                {
                    str += ", ";
                }
                str += values[i] + " x " + frequencies[i];
            }

            return str;
        }
    }

    class MedianSummary
    {
        public float min, lowerQ, median, upperQ, max;

        public MedianSummary(float _min, float _lowerQ, float _median, float _upperQ, float _max)
        {
            min = _min;
            lowerQ = _lowerQ;
            median = _median;
            upperQ = _upperQ;
            max = _max;
        }
        public MedianSummary() { }
    }

    class MeanSummary
    {
        public float lowerBound, mean, upperBound;
        public float sd;

        public MeanSummary(float _lowerBound, float _mean, float _upperBound, float _sd)
        {
            lowerBound = _lowerBound;
            mean = _mean;
            upperBound = _upperBound;
            sd = _sd;
        }
        public MeanSummary() { }
    }

    class TrendSummary
    {
        public int quality;
        public float[] means;
        public DateTime[] dates;

        public TrendSummary(float[] _means, DateTime[] _dates, int _quality)
        {
            means = _means;
            dates = _dates;
            quality = _quality;
        }
        public TrendSummary() { }
    }

    class ErrorData
    {
        public string description;
        internal Collection<string[]> stringArrays = new Collection<string[]>();
        internal Collection<string> strings = new Collection<string>();
        public Exception exception;
        public int count;
        public string log;

        public ErrorData(Exception _exception, string _decription)
        {
            description = _decription;
            exception = _exception;
            count = 1;
        }
        public ErrorData(string _decription)
        {
            description = _decription;

            stringArrays = new Collection<string[]>();
            count = 1;
        }
    }
}
