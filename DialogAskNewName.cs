using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace nando
{
    public partial class DialogNewName : Form
    {
        public DialogNewName()
        {
            InitializeComponent();
        }

        public void SetQuestion(string question)
        {
            labelQuestion.Text = question;
        }

        public void SetName(string newname)
        {
            name.Text = newname;
        }

        public string getAnswer()
        {
            return name.Text;
        }
    }
}
