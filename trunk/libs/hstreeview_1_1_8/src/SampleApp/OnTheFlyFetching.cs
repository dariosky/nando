using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace SampleApp
{
    public partial class OnTheFlyFetching : UserControl
    {
        public OnTheFlyFetching()
        {
            InitializeComponent();
            OnTheFlyModel model = new OnTheFlyModel();
            model.Enumerate += Model_Enumerate;
            treeViewAdv1.Model = model;
        }

        private void Model_Enumerate(object sender, EventArgs e)
        {
            lblLastFetched.Text = (sender as OnTheFlyModel).Current.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            treeViewAdv1.ExpandAll();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            treeViewAdv1.CollapseAll();
        }
    }
}
