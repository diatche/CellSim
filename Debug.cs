using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Collections.ObjectModel;

namespace AI
{
    /// <summary>
    /// Displays errors real-time.
    /// </summary>
    class Debug
    {
        F f;
        Form form;
        TextBox textbox;
        public string version = "1.3";
        Collection<ErrorData> errors = new Collection<ErrorData>();
        bool disposed;

        //make a collectiong of strings. each error string will
        //have a counter (to prevent flooding).
        //create a form and list all the errors in a textbox.

        //optionaly save the error log to a file.
        public Debug()
        {
            f = new F();

            //initialise form
            form = new System.Windows.Forms.Form();
            form.Name = "Form";
            form.Text = "Debug";
            form.Size = new System.Drawing.Size(500, 400);
            form.Disposed += new EventHandler(form_Disposed);
            disposed = false;

            form.Visible = false;

            //initialise textbox
            textbox = new TextBox();
            textbox.Parent = form;
            textbox.Text = "";
            textbox.Multiline = true;
            textbox.ScrollBars = ScrollBars.Both;
            textbox.WordWrap = false;
            textbox.Dock = DockStyle.Fill;
        }

        void form_Disposed(object sender, EventArgs e)
        {
            disposed = true;
        }

        public void NewError(ErrorData errorData)
        {
            if (disposed)
            {
                return;
            }

            //check if error exists
            int c = errors.Count;
            if (c != 0)
            {
                for (int i = 0; i < c; i++)
                {
                    if (errorData.exception.Message == errors[i].exception.Message)
                    {
                        //error exists, add count
                        if (errors[i].count == 100)
                        {
                            errors[i].count = int.MaxValue;
                            Update();
                            return;
                        }
                        else if (errors[i].count > 100)
                        {
                            return;
                        }
                        else
                        {
                            errors[i].count++;
                            Update();
                            return;
                        }
                    }
                }
            }

            //write log
            string log, tab;

            tab = "     ";

            //decription
            log = DateTime.Now + ". Version " + version + Environment.NewLine;
            log += tab + errorData.description + Environment.NewLine;
            //exception
            if (errorData.exception != null)
            {
                if (errorData.exception.Message != null)
                {
                    log += tab + errorData.exception.Message + Environment.NewLine;
                }
                log += tab + errorData.exception.ToString() + Environment.NewLine;
                if (errorData.exception.HelpLink != null)
                {
                    log += tab + errorData.exception.HelpLink + Environment.NewLine;
                }
            }
            //data
            c = errorData.stringArrays.Count;
            if (c != 0)
            {
                for (int i = 0; i < c; i++)
                {
                    log += tab + f.ToString(errorData.stringArrays[i], "|") + Environment.NewLine;
                }
            }
            c = errorData.strings.Count;
            if (c != 0)
            {
                for (int i = 0; i < c; i++)
                {
                    log += tab + errorData.strings[i] + Environment.NewLine;
                }
            }

            //portal.AppendData(log, file_error);
            errorData.log = log;

            errors.Add(errorData);

            //-------
            Update();
        }

        private void Update()
        {
            if (disposed)
            {
                return;
            }

            int c = errors.Count;
            if (c == 0)
            {
                form.Visible = false;
                return;
            }

            string str = "";

            //list all the errors' logs
            for (int i = 0; i < c; i++)
            {
                if (errors[i].count == int.MaxValue)
                {
                    str += "x Infinity" + Environment.NewLine;
                }
                else
                {
                    str += "x" + errors[i].count + Environment.NewLine;
                }
                str += errors[i].log;
            }

            textbox.Text = str;

            form.Visible = true;
        }
    }
}
