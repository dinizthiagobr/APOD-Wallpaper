using AstronomyPicOfTheDay.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AstronomyPicOfTheDay
{
    public partial class ExplicationForm : Form
    {
        public ExplicationForm(PictureOfTheDay p)
        {
            InitializeComponent();

            if (p == null)
                titleLabel.Text = "ERRO AO CARREGAR FOTO";
            else
            {
                titleLabel.Text = p.title;
                dateLabel.Text = p.date;
                explanationTextBox.Text = p.explanation;
            }
        }
    }
}
