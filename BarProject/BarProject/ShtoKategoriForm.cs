﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BarProject
{
    public partial class ShtoKategoriForm : Form
    {
        public ShtoKategoriForm()
        {
            InitializeComponent();
        }

        private void ZgjidhFotoButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.Title = "Zgjidh Foto...";
            ofd.DefaultExt = ".jpg";
            ofd.Filter = "Media Files|*.jpg;*.png;*.gif;*.bmp;*.jpeg|All Files|*.*";

            DialogResult result = ofd.ShowDialog();

            if (result == DialogResult.OK)
            {
                FotoBox.Load(ofd.FileName); 
            }
        }

        private void ShtoKategoriButton_Click(object sender, EventArgs e)
        {
            DataController dataController = new DataController();

            string emer = EmriBox.Text;
            string pershkrimi = PershkrimBox.Text;
            PictureBox foto = FotoBox;


            if (dataController.shtoKategori(emer, pershkrimi, foto))
            {
                MessageBox.Show("Kategoria u shtua");
            }
            else
                MessageBox.Show("Kategoria nuk u shtua!");
        }

    }
}