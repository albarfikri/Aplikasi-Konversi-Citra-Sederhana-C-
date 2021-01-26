using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjectPCD
{
    public partial class Form1 : Form
    {
        Bitmap newBitmap;
        Image file;
        Boolean opened = false;
        int blurAmount = 0;
        float gamma = 0;
        int lastCol = 0;

        public Form1()
        {
            InitializeComponent();
        }

        //Scrolling Brightness
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            label2.Text = trackBar1.Value.ToString();
            pictureBox1.Image = AdjustBrightness(newBitmap, trackBar1.Value);

        }

        //Brightness

        public static Bitmap AdjustBrightness(Bitmap Image, int Value)
        {
         
                //Proses pertama masukan gambar menjadi sebuah bitmap
                Bitmap TempBitmap = Image;

                //setelah itu nilai dari gambar dibagi dengan nilai maximum dari rgb yaitu 255
                float FinalValue = (float)Value / 255.0f;

                //Setelah proses selesai masukan kedalam new bitmap
                Bitmap NewBitmap = new Bitmap(TempBitmap.Width, TempBitmap.Height);

                Graphics NewGraphics = Graphics.FromImage(NewBitmap);

                float[][] FloatColorMatrix = {
                new float[] {1,0,0,0,0},
                new float[] {0,1,0,0,0},
                new float[] {0,0,1,0,0},
                new float[] {0,0,0,1,0},
                new float[] {FinalValue, FinalValue, FinalValue, 1, 1}

            };
                System.Drawing.Imaging.ColorMatrix NewColorMatrix = new ColorMatrix(FloatColorMatrix);

                System.Drawing.Imaging.ImageAttributes Attributes = new ImageAttributes();

                Attributes.SetColorMatrix(NewColorMatrix);

                NewGraphics.DrawImage(TempBitmap, new Rectangle(0, 0, TempBitmap.Width, TempBitmap.Height), 0, 0, TempBitmap.Width, TempBitmap.Height, GraphicsUnit.Pixel, Attributes);

                Attributes.Dispose();

                NewGraphics.Dispose();

                return NewBitmap;
            
        }

        private void OpenButton_Click(object sender, EventArgs e)
        {
            DialogResult dr = openFileDialog1.ShowDialog();

            if (dr == DialogResult.OK)
            {
                file = Image.FromFile(openFileDialog1.FileName);
                newBitmap = new Bitmap(openFileDialog1.FileName);
                pictureBox1.Image = file;
                opened = true;
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {

            if (opened == true)
            {
                DialogResult dr = saveFileDialog1.ShowDialog();
                if (dr == DialogResult.OK)
                {

                    if (saveFileDialog1.FileName.Substring(saveFileDialog1.FileName.Length - 3).ToLower() == "bmp")
                    {
                        newBitmap.Save(saveFileDialog1.FileName, ImageFormat.Bmp);
                    }
                    if (saveFileDialog1.FileName.Substring(saveFileDialog1.FileName.Length - 3).ToLower() == "jpg")
                    {
                        newBitmap.Save(saveFileDialog1.FileName, ImageFormat.Jpeg);
                    }
                    if (saveFileDialog1.FileName.Substring(saveFileDialog1.FileName.Length - 3).ToLower() == "png")
                    {
                        newBitmap.Save(saveFileDialog1.FileName, ImageFormat.Png);

                    }
                    if (saveFileDialog1.FileName.Substring(saveFileDialog1.FileName.Length - 3).ToLower() == "tif")
                    {
                        newBitmap.Save(saveFileDialog1.FileName, ImageFormat.Tiff);
                    }
                }

            }
            else
            {
                MessageBox.Show("Gambarnya dibuka dulu yaah !");
            }

        }


        private void GrayscaleButton_Click(object sender, EventArgs e)
        {
            //proses mengecek seluruh pixel dari gambar pada sumbu x
            for (int x = 0; x < newBitmap.Width; x++)
            {
                //proses mengecek seluruh pixel dari gambar pada sumbu y
                for (int y = 0; y < newBitmap.Height; y++)
                {
                    //berfungsi mengambil warna asli dari citra
                    Color asli = newBitmap.GetPixel(x, y);

                    //menkonversi setiap unsur warna dari pixel yaitu rgb
                    int grayScale = (int)((asli.R * .3) + (asli.G * .59) + (asli.B * .11));

                    Color hasil = Color.FromArgb(grayScale, grayScale, grayScale);

                    newBitmap.SetPixel(x, y, hasil);
                }
            }
            pictureBox1.Image = newBitmap;
        }

        private void BlurButton_Click(object sender, EventArgs e)
        {
            //proses mengecek seluruh pixel dari gambar pada sumbu x
            for (int x = blurAmount; x <= newBitmap.Width - blurAmount; x++)
            {
                //proses mengecek seluruh pixel dari gambar pada sumbu y
                for (int y = blurAmount; y <= newBitmap.Height - blurAmount; y++)
                {
                    //mencoba line sehingga in case ada kesalahan maka program tidak langsung akan berhenti
                    try
                    {
                        Color prevX = newBitmap.GetPixel(x - blurAmount, y);
                        Color nextX = newBitmap.GetPixel(x + blurAmount, y);
                        Color prevY = newBitmap.GetPixel(x, y - blurAmount);
                        Color nextY = newBitmap.GetPixel(x, y + blurAmount);

                        //avg stands for average atau rata rata 

                        int avgR = (int)((prevX.R + nextX.R + prevY.R + nextY.R) / 4);
                        int avgG = (int)((prevX.G + nextX.G + prevY.G + nextY.G) / 4);
                        int avgB = (int)((prevX.B + nextX.B + prevY.B + nextY.B) / 4);


                        newBitmap.SetPixel(x, y, Color.FromArgb(avgR, avgG, avgB));

                    }
                    catch (Exception)
                    {

                    }
                }

            }
            pictureBox1.Image = newBitmap;
        }


        private void updateBlur(object sender, EventArgs e)
        {
            blurAmount = int.Parse(trackBar2.Value.ToString());

        }

        private void InvertionButton_Click(object sender, EventArgs e)
        {
            //proses mengecek seluruh pixel dari gambar pada sumbu x
            for (int x = 0; x < newBitmap.Width; x++)
            {
                //proses mengecek seluruh pixel dari gambar pada sumbu y
                for (int y = 0; y < newBitmap.Height; y++)
                {
                    Color pixel = newBitmap.GetPixel(x, y);
                    int red = pixel.R;
                    int green = pixel.G;
                    int blue = pixel.B;


                    //saat melakukan proses inversi maka jika nilai dari salah satu pixel 100 maka dikurang dengan nilai
                    //maksimumnya yaitu 255 - 100, lakukan hal yang sama untuk setiap pixel dan jenisnya.
                    newBitmap.SetPixel(x, y, Color.FromArgb(255 - red, 255 - green, 255 - blue));
                }
            }
            pictureBox1.Image = newBitmap;
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            label6.Text = trackBar3.Value.ToString();

            gamma = 0.04f * trackBar3.Value;

            Bitmap bm = new Bitmap(newBitmap.Width, newBitmap.Height);

            Graphics g = Graphics.FromImage(bm);

            ImageAttributes ia = new ImageAttributes();

            ia.SetGamma(gamma);

            g.DrawImage(newBitmap, new Rectangle(0, 0, newBitmap.Width, newBitmap.Height), 0, 0, newBitmap.Width, newBitmap.Height, GraphicsUnit.Pixel, ia);
            //berfungsi untuk melepaskan atau membersihkan objek
            g.Dispose();
            ia.Dispose();
            pictureBox1.Image = bm;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult obj = MessageBox.Show("yakin ingin ke tampilan gambar normal ?","Konfirmasi",MessageBoxButtons.YesNo);
            if(obj==DialogResult.Yes)
            {
                pictureBox1.Image = file;
                newBitmap = (Bitmap)file;
                label2.Text = "0";
                trackBar1.Value = 0;
                trackBar2.Value = 0;
                trackBar3.Value = 0;

                MessageBox.Show("Berhasil kembali tampilan normal !");
            }
            else
            {

            }
           
        }

        private void emboss_Click(object sender, EventArgs e)
        {
            //embooss atau membuat gambar seolah olah diukir
            Bitmap nB = new Bitmap(newBitmap.Width, newBitmap.Height);

            for (int x = 1; x <= newBitmap.Width - 1; x++){
               for(int y = 1; y<=newBitmap.Height - 1; y++)
                {
                    nB.SetPixel(x, y, Color.DarkGray);
                }
            }

            for(int x = 1; x <= newBitmap.Width - 1; x++)
            {
                for(int y = 1; y<=newBitmap.Height -1; y++)
                {
                    try
                    {
                        Color pixel = newBitmap.GetPixel(x, y);
                        // membuat sebuah variable colVal kemudian memasukan warna dari setiap pixel
                        int colVal = (pixel.R + pixel.G + pixel.B);

                        if (lastCol == 0) lastCol = (pixel.R + pixel.G + pixel.B);

                        int diff;
                        if (colVal > lastCol) { diff = colVal - lastCol; } else { diff = lastCol - colVal; }
                        if(diff>100)
                        {
                            nB.SetPixel(x, y, Color.Blue);
                            lastCol = colVal;
                        }
                    }
                    catch (Exception) { }
                }
            }

            for (int y = 1; y <= newBitmap.Height - 1; y++)
            {
                for (int x = 1; x <= newBitmap.Width - 1; x++)
                {
                    try
                    {
                        Color pixel = newBitmap.GetPixel(x, y);
                        // membuat sebuah variable colVal kemudian memasukan warna dari setiap pixel
                        int colVal = (pixel.R + pixel.G + pixel.B);

                        if (lastCol == 0) lastCol = (pixel.R + pixel.G + pixel.B);

                        int diff;
                        if (colVal > lastCol) { diff = colVal - lastCol; } else { diff = lastCol - colVal; }
                        if (diff > 100)
                        {
                            nB.SetPixel(x, y, Color.Blue);
                            lastCol = colVal;
                        }
                    }
                    catch (Exception) { }
                }
            }
            pictureBox1.Image = nB;
        }
    }
}

