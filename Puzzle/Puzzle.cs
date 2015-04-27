using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Puzzle
{
    public partial class Puzzle : Form
    {
        //图片位置字典
        Dictionary<PictureBox, Point> pictureLocationDict = new Dictionary<PictureBox, Point>();

        //所拖拽的图片
        PictureBox currentPictureBox = null;
        //被迫需要移动的图片
        PictureBox haveToPictureBox = null;
        //原位置
        Point oldLocation = Point.Empty;
        //新位置
        Point newLocation = Point.Empty;
        //鼠标按下坐标（control控件的相对坐标） 
        Point mouseDownPoint = Point.Empty;
        //显示拖动效果的矩形 
        Rectangle rect = Rectangle.Empty;
        //是否正在拖拽 
        bool isDrag = false;

        public Puzzle()
        {
            InitializeComponent();
            pictureLocationDict.Add(pictureBox1, pictureBox1.Location);
            pictureLocationDict.Add(pictureBox2, pictureBox2.Location);
            pictureLocationDict.Add(pictureBox3, pictureBox3.Location);
            pictureLocationDict.Add(pictureBox4, pictureBox4.Location);
            pictureLocationDict.Add(pictureBox5, pictureBox5.Location);
            pictureLocationDict.Add(pictureBox6, pictureBox6.Location);
            pictureLocationDict.Add(pictureBox7, pictureBox7.Location);
            pictureLocationDict.Add(pictureBox8, pictureBox8.Location);
            pictureLocationDict.Add(pictureBox9, pictureBox9.Location);
        }

        private void Puzzle_Paint(object sender, PaintEventArgs e)
        {
            if (rect != Rectangle.Empty)
            {
                if (isDrag)
                {
                    e.Graphics.DrawRectangle(Pens.White, rect);
                }
                else
                {
                    e.Graphics.DrawRectangle(new Pen(this.BackColor), rect);
                }
            }
        }

     
        /// <summary>
        /// 不好用
        /// </summary>
        /// <returns></returns>
        public PictureBox GetPictureBoxByLocation()
        {
            PictureBox pic = null;
            if (this.ActiveControl.Name.Contains("pictureBox"))
            {
                pic = (PictureBox)this.ActiveControl;
            }
            return pic;

        }

        public PictureBox GetPictureBoxByLocation(MouseEventArgs e)
        {
            PictureBox pic = null;
            foreach (PictureBox item in pictureLocationDict.Keys)
            {
                if (e.Location.X > item.Location.X && e.Location.Y > item.Location.Y && item.Location.X + 100 > e.Location.X && item.Location.Y + 100 > e.Location.X)
                {
                    pic = item;
                }
            }
            return pic;
        }

        public PictureBox GetPictureBoxByLocation(int x,int y)
        {
            PictureBox pic = null;
            foreach (PictureBox item in pictureLocationDict.Keys)
            {
                if (x> item.Location.X && y > item.Location.Y && item.Location.X + 100 > x && item.Location.Y + 100 > y)
                {
                    pic = item;
                }
            }
            return pic;
        }

        /// <summary>
        /// 通过hashcode获取picture，用mouseeventargs之后获取相对于picture的坐标不是相对窗体
        /// </summary>
        /// <param name="hascode"></param>
        /// <returns></returns>
        public PictureBox GetPictureBoxByHashCode(string hascode)
        {
            PictureBox pic = null;
            foreach (PictureBox item in pictureLocationDict.Keys)
            {
                if (hascode == item.GetHashCode().ToString())
                {
                    pic = item;
                }
            }
            return pic;
        }

        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            oldLocation = new Point(e.X, e.Y);
            currentPictureBox = GetPictureBoxByHashCode(sender.GetHashCode().ToString());
            MoseDown(currentPictureBox, sender, e);
        }

        public void MoseDown(PictureBox pic, object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                oldLocation = e.Location;
                rect = pic.Bounds;
            }
        }

        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDrag = true;
                rect.Location = getPointToForm(new Point(e.Location.X - oldLocation.X, e.Location.Y - oldLocation.Y));
                this.Refresh();

            }
        }

       

        private void reset()
        {
            mouseDownPoint = Point.Empty;
            rect = Rectangle.Empty;
            isDrag = false;
        }

        private Point getPointToForm(Point p)
        {
            return this.PointToClient(pictureBox1.PointToScreen(p));
        }

        private void pictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            oldLocation = new Point(currentPictureBox.Location.X, currentPictureBox.Location.Y);
            if (oldLocation.X + e.X > 300 || oldLocation.Y + e.Y > 300||oldLocation.X + e.X < 0 || oldLocation.Y + e.Y < 0)
            {
                return;
            }
            haveToPictureBox = GetPictureBoxByLocation(oldLocation.X + e.X, oldLocation.Y + e.Y);
            newLocation = new Point(haveToPictureBox.Location.X, haveToPictureBox.Location.Y);
            haveToPictureBox.Location = oldLocation;
            PictureMouseUp(currentPictureBox, sender, e);
        }

        public void PictureMouseUp(PictureBox pic, object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (isDrag)
                {
                    isDrag = false;
                    pic.Location = newLocation;
                    this.Refresh();
                }
                reset();
            }
        }

        public void ExchangePictureBox(MouseEventArgs e)
        { }

        private void btn_sta_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this.ActiveControl.Name);
        }



    }
}
