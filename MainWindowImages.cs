using System;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;

namespace Migracja
{
    public partial class MainWindow : Form
    {
        enum MovedPicture { source, desination };

        private void StartMovingPictures()
        {
            blMouseInMoveMode = true;
            mouseDownSourcePBLeft = sourcePB.Left;
            mouseDownSourcePBTop = sourcePB.Top;
            mouseDownDesinationPBLeft = destinationPB.Left;
            mouseDownDesinationPBTop = destinationPB.Top;
        }

        private void StopMovingPictures(MovedPicture picture)
        {
            float dpShift = float.Parse(ScaleTB.Text);
            //jeśli nie odbyło się przesunięcie obrazka, to wyświetlamy dane odnośnie odpowiedniego vectorRectangle
            if (mouseDownDesinationPBLeft == destinationPB.Left && mouseDownDesinationPBTop == destinationPB.Top)
            {
                
            }
            //nastąpiło przesunięcie obrazka
            else
            {
                switch (picture)
                {
                        //centrum oglądanego obszaru przesówamy o przesunięcię myszą z uwzględnieniem sklai
                    case MovedPicture.source:
                        {
                            sourceImageCropper.centerX +=
                                (int) Math.Round((mouseDownSourcePBLeft - sourcePB.Left)/dpShift);
                            sourceImageCropper.centerY +=
                                (int) Math.Round((mouseDownSourcePBTop - sourcePB.Top)/dpShift);
                            desinationImageCrooper.centerX = sourceImageCropper.centerX;
                            desinationImageCrooper.centerY = sourceImageCropper.centerY;
                            break;
                        }
                    case MovedPicture.desination:
                        {
                            desinationImageCrooper.centerX +=
                                (int) Math.Round((mouseDownDesinationPBLeft - destinationPB.Left)/dpShift);
                            desinationImageCrooper.centerY +=
                                (int) Math.Round((mouseDownDesinationPBTop - destinationPB.Top)/dpShift);
                            sourceImageCropper.centerX = desinationImageCrooper.centerX;
                            sourceImageCropper.centerY = desinationImageCrooper.centerY;
                            break;
                        }
                }
                windowSettings.centerX = sourceImageCropper.centerX;
                windowSettings.centerY = sourceImageCropper.centerY;
                DrawCroppedScaledImage(dpShift, UpdateInfoBoxTime);
            }
            blMouseInMoveMode = false;
        }

        private void MovePictures(MouseEventArgs e)
        {
            if (blMouseInMoveMode)
            {
                horChange = e.X - startingX;
                verChange = e.Y - startingY;

                sourcePB.Left = Math.Min(0, Math.Max(sourcePB.Left + horChange, sourcePanel.Width - sourcePB.Image.Width));
                sourcePB.Top = Math.Min(0, Math.Max(sourcePB.Top + verChange, sourcePanel.Height - sourcePB.Image.Height));
                destinationPB.Left = Math.Min(0, Math.Max(destinationPB.Left + horChange, destinationPanel.Width - destinationPB.Image.Width));
                destinationPB.Top = Math.Min(0, Math.Max(destinationPB.Top + verChange, destinationPanel.Height - destinationPB.Image.Height));
                UpdateInfoBox("startingX: " + startingX.ToString() + "\n" +
                              "startingY: " + startingY.ToString() + "\n" +
                              "horChange: " + horChange.ToString() + "\n" +
                              "verChange" + verChange.ToString() + "\n" +
                              "e.X" + e.X.ToString() + "\n" +
                              "e.Y" + e.Y.ToString());
            }
            else
            {
                startingX = e.X;
                startingY = e.Y;
            }

        }

        private void SetScaleControlEnable(bool aEnabled){
            ZoomInBtn.Enabled = aEnabled;
            ZoomOutBtn.Enabled = aEnabled;
            ScaleTrB.Enabled = aEnabled;
        }

        //metoda po wczytaniu pliku save inicjalizuje obraz źródłowy i czyści docelowy. 
        //Buduje na nowo też obiekty sourceImageCropper i destinationImageCropper
        private void PrepareSourceImage(String aPath)
        {
            sourceBmp = new Bitmap(aPath);
            destinationBmp = null;
            sourceImageCropper = new RaserImageCrooper(new Size(sourcePanel.Width, sourcePanel.Height), sourceBmp);
            desinationImageCrooper = new VectorImageCrooper(new Size(destinationPanel.Width, destinationPanel.Height), mapFactory,
                                                            sourceImageCropper.centerX, sourceImageCropper.centerY, windowSettings,
                                                            sourceBmp);
        }

        private bool LoadImage(String aPath)
        {
            if (File.Exists(aPath))
            {
                PrepareSourceImage(aPath);
                DrawCroppedScaledImage(float.Parse(ScaleTB.Text), UpdateInfoBoxTime);
                SetScaleControlEnable(true);
                return true;
            }
            return false;
        }

        private bool DrawCroppedScaledImage(float aDpScale,  UpdateInfoBoxTimeDelegate aFunct = null, float? aDpScalePrev = null)
        {
            DateTime dtTimePrv = DateTime.Now;
            Bitmap croppedSrcBmp = sourceImageCropper.GetCroppedImage(aDpScale);
            dtTimePrv = aFunct("GetCroppedImage 1:", true, dtTimePrv);
            //tymczasowo przypisuję ten sam obraz
            //Bitmap croppedDstBmp = croppedSrcBmp;
            Bitmap croppedDstBmp;
            if (desinationImageCrooper.mapFactory == null)
            {
                croppedDstBmp = null;
                dtTimePrv = aFunct("GetCroppedImage 2 NULL:", false, dtTimePrv);
            }
            else
            {
                croppedDstBmp = desinationImageCrooper.GetCroppedImage(aDpScale, aFunct);
                dtTimePrv = aFunct("GetCroppedImage 2:", false, dtTimePrv);
            }
            
            /*int scaledShiftX = sourcePanel.Width;
            int scaledShiftY = sourcePanel.Height;*/
            int scaledShiftX = (int)Math.Round(sourceImageCropper.centerX * aDpScale);
            int scaledShiftY = (int)Math.Round(sourceImageCropper.centerY * aDpScale);

            //UpdateInfoBox("bmp: " + croppedSrcBmp.Width.ToString() + " x " + croppedSrcBmp.Height.ToString());
            sourcePB.Height = croppedSrcBmp.Height;
            sourcePB.Width = croppedSrcBmp.Width;
            sourcePB.Image = croppedSrcBmp;
            //UpdateInfoBox("pb: " + sourcePB.Width.ToString() + " x " + sourcePB.Height.ToString() +
            //              "L/T: " + sourcePB.Left.ToString() + " x " + sourcePB.Top.ToString());
            /*sourcePB.Left = -Math.Min(scaledShiftX, sourcePanel.Width);
            sourcePB.Top = -Math.Min(scaledShiftY, sourcePanel.Height);*/
            sourcePB.Left = -sourcePanel.Width;
            sourcePB.Top = -sourcePanel.Height;
            //UpdateInfoBox("L/T: " + sourcePB.Left.ToString() + " x " + sourcePB.Top.ToString(), false);

            if (croppedDstBmp != null)
            {
                destinationPB.Height = croppedDstBmp.Height;
                destinationPB.Width = croppedDstBmp.Width;
                destinationPB.Image = croppedDstBmp;
                /*destinationPB.Left = -Math.Min(scaledShiftX, destinationPanel.Width);
                destinationPB.Top = -Math.Min(scaledShiftY, destinationPanel.Height);*/
                destinationPB.Left = -destinationPanel.Width;
                destinationPB.Top = -destinationPanel.Height;
            }
            aFunct("Pozostałe:", false, dtTimePrv);
            return true;
        }

        private void LoadImage()
        {
            if (loadDialog.ShowDialog() == DialogResult.OK)
            {
                LoadImage(loadDialog.FileName);
                windowSettings.stSourceImagePath = loadDialog.FileName;
            }
        }

        private void ReloadImage(){
            if (windowSettings.stSourceImagePath != "")
            {
                if (File.Exists(windowSettings.stSourceImagePath))
                    LoadImage(windowSettings.stSourceImagePath);
                else
                {

                }
            }
            else
            {
                
            }
        }

        private void GetVectRectData(MouseEventArgs args)
        {
            int i = 0;
            Point tmpPoint = new Point(args.X, args.Y);
            i = 9;
//            UpdateInfoBox("Punkt wewnątrz PB: (" + (Math.Round((double)(tmpPoint.X))).ToString() + ", " +
//                                                   (Math.Round((double)(tmpPoint.Y))).ToString() + ")");
//            UpdateInfoBox("Punkt PB LT: (" + (Math.Round((double)(destinationPB.Left))).ToString() + ", " +
//                                             (Math.Round((double)(destinationPB.Top))).ToString() + ")");
//            UpdateInfoBox("Punkt wewnątrz PB: (" + (Math.Round((double)(tmpPoint.X))).ToString() + ", " +
//                                               (Math.Round((double)(tmpPoint.Y))).ToString() + ")");
//            UpdateInfoBox("Punkt ImgShift: (" + (Math.Round((double)(desinationImageCrooper.srcShiftX))).ToString() + ", " +
//                                                (Math.Round((double)(desinationImageCrooper.srcShiftY))).ToString() + ")");
            int imgX = (int)Math.Floor(
                                        (tmpPoint.X - desinationImageCrooper.srcShiftX)/windowSettings.dpScale
                                      );
            int imgY = (int)Math.Floor(
                                        (tmpPoint.Y - desinationImageCrooper.srcShiftY) / windowSettings.dpScale
                                      );

            UpdateInfoBox("Punkt scaled destinationPB: (" + imgX.ToString() + ", " + imgY.ToString() + ")");
            Vector_Rectangle vectRect = mapFactory.vectArr[imgX][imgY];
            Dictionary<int, VectorRectangleEdgePoint> tmpList = vectRect.parentVectorRectangleEdgePointList;
            if (tmpList != null && tmpList.Count > 0)
            {
                String msg = "";
                EdgeSlice edgeSlice;
                if (tmpList.ContainsKey(Dir.fromLeftToRight))
                {
                    edgeSlice = tmpList[Dir.fromLeftToRight].edgeSlice;
                    msg = msg + Cst.NL + "Fragment fromLeftToRight " + edgeSlice.Info();
                };
                if (tmpList.ContainsKey(Dir.fromTopToBottom))
                {
                    edgeSlice = tmpList[Dir.fromTopToBottom].edgeSlice;
                    msg = msg + Cst.NL + "Fragment fromTopToBottom " + edgeSlice.Info();
                };
                if (tmpList.ContainsKey(Dir.fromRightToLeft))
                {
                    edgeSlice = tmpList[Dir.fromRightToLeft].edgeSlice;
                    msg = msg + Cst.NL + "Fragment fromRightToLeft " + edgeSlice.Info();
                };
                if (tmpList.ContainsKey(Dir.fromBottomToTop))
                {
                    edgeSlice = tmpList[Dir.fromBottomToTop].edgeSlice;
                    msg = msg + Cst.NL + "Fragment fromBottomToTop " + edgeSlice.Info();
                };
                UpdateInfoBox(msg);
            }
            else
            {
                UpdateInfoBox("Punkt nie należy do żadnego fragmentu granicy.");
            }
        }
    }
}
