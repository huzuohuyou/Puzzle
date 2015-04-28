using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Puzzle
{
    public partial class Puzzle : Form
    {
        //图片列表
        PictureBox[] pictureList = null;
        //图片位置字典
        SortedDictionary<string, Bitmap> pictureLocationDict = new SortedDictionary<string, Bitmap>();
        //Location List 
        Point[] pointList = null;
        //图片控件字典
        SortedDictionary<string, PictureBox> pictureBoxLocationDict = new SortedDictionary<string, PictureBox>();
        //拼图时间
        int second = 0;
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
            InitGame();
        }

        /// <summary>
        /// 初始化游戏资源
        /// </summary>
        public void InitGame()
        {
            pictureList = new PictureBox[9] { pictureBox1, pictureBox2, pictureBox3, pictureBox4, pictureBox5, pictureBox6, pictureBox7, pictureBox8, pictureBox9 };
            pointList = new Point[9] { new Point(0, 0), new Point(100, 0), new Point(200, 0), new Point(0, 100), new Point(100, 100), new Point(200, 100), new Point(0, 200), new Point(100, 200), new Point(200, 200) };
            if (!Directory.Exists(Application.StartupPath.ToString() + "\\Picture"))
            {
                Directory.CreateDirectory(Application.StartupPath.ToString() + "\\Picture");
                Properties.Resources.默认.Save(Application.StartupPath.ToString() + "\\Picture\\默认.jpg");
            }
            Flow(Application.StartupPath.ToString() + "\\Picture\\默认.jpg");
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
            foreach (PictureBox item in pictureList)
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
            foreach (PictureBox item in pictureList)
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
            foreach (PictureBox item in pictureList)
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
            if ( Judge())
            {
                lab_result.Text = "成功";
                //MessageBox.Show("恭喜拼图成功");
            }
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

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="path"></param>
        public void Flow(string path)
        {
            Image bm = CutPicture.Resize(path, 300, 300);
            CutPicture.BitMapList = new List<Bitmap>();
            for (int y = 0; y < 300; y += 100)
            {
                for (int x = 0; x < 300; x += 100)
                {
                    //string key = x + "-" + y;
                    Bitmap temp = CutPicture.Cut(bm, x, y, 100, 100);
                    //pictureLocationDict.Add(key, temp);
                    CutPicture.BitMapList.Add(temp);
                }
            }
            ImportBitMap();
        }

        /// <summary>
        /// 打乱数据
        /// </summary>
        /// <param name="pictureArray"></param>
        /// <returns></returns>
        public PictureBox[] DisOrderArray(PictureBox[] pictureArray)
        {
            PictureBox[] tempArray = pictureArray;
            for (int i = tempArray.Length - 1; i > 0; i--)
            {
                Random rand = new Random();
                int p = rand.Next(i);
                PictureBox temp = tempArray[p];
                tempArray[p] = tempArray[i];
                tempArray[i] = temp;
            }
            return tempArray;
        }

        /// <summary>
        /// 判断是否拼图成功
        /// </summary>
        /// <returns></returns>
        public bool Judge()
        {
            bool result = true;
            int i = 0;
            foreach (PictureBox item in pictureList)
            {
                if (item.Location != pointList[i])
                {
                    result = false;
                }
                i++;
            }
            return result;
        }

        private void btn_import_Click(object sender, EventArgs e)
        {
            ofd_picture.ShowDialog();
            CutPicture.PicturePath = ofd_picture.FileName;
            Flow(CutPicture.PicturePath);
            CountTime();
        }

        /// <summary>
        /// 计时
        /// </summary>
        public void CountTime()
        {
            lab_time.Text = "0";
            timer1.Start();
        }

        /// <summary>
        /// 给piturebox赋值
        /// </summary>
        public void ImportBitMap()
        {
            try
            {

                int i = 0;// DisOrderArray(pictureList)
                foreach (PictureBox item in pictureList)
                {
                    Bitmap temp = CutPicture.BitMapList[i];
                    item.Image = temp;
                    i++;
                }
                ResetPictureLocation();
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);
            }
            
        }

        /// <summary>
        /// 打乱位置列表
        /// </summary>
        /// <returns></returns>
        public Point[] DisOrderLocation()
        {
            Point[] tempArray = (Point[])pointList.Clone();
            for (int i = tempArray.Length - 1; i > 0; i--)
            {
                Random rand = new Random();
                int p = rand.Next(i);
                Point temp = tempArray[p];
                tempArray[p] = tempArray[i];
                tempArray[i] = temp;
            }
            return tempArray;
        }

        /// <summary>
        /// 重新设置图片位置
        /// </summary>
        public void ResetPictureLocation()
        {
            Point[] temp = DisOrderLocation();
            int i = 0;
            foreach (PictureBox item in pictureList)
            {
                item.Location = temp[i];
                i++;
            }
        }

        /// <summary>
        /// 计时，超过30秒停止计时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            second++;
            lab_time.Text = second.ToString();
            if (second == 30)
            {
                timer1.Stop();
                lab_result.Text = "失败！";
            }
        }

        private void btn_sta_Click_1(object sender, EventArgs e)
        {
            timer1.Start();
        }



    }
}
