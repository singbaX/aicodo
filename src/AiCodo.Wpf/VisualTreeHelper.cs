// Copyright (c) 2021 AiCodo.com Corporation. All Rights Reserved.
// Licensed under the MIT License.

namespace AiCodo
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Text;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    public static class Visual_TreeHelper
    {
        public static T FindVisualRootParent<T>(this DependencyObject obj) where T : DependencyObject
        {
            if (obj != null)
            {
                DependencyObject loopObj = obj;
                while (true)
                {
                    DependencyObject d = VisualTreeHelper.GetParent(loopObj);
                    if (d == null)
                    {
                        return (T)obj;
                    }
                    else
                        loopObj = d;
                }
            }
            return null;
        }

        public static T FindVisualChildItem<T>(this DependencyObject obj) where T : DependencyObject
        {
            if (null != obj)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                    if (child != null && child is T)
                        return (T)child;
                    else
                    {
                        T childOfChild = FindVisualChildItem<T>(child);
                        if (childOfChild != null)
                            return childOfChild;
                    }
                }
            }
            return null;
        }

        public static T FindVisualChildItem<T>(this DependencyObject obj, string name) where T : FrameworkElement
        {
            if (null != obj)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                    if (child != null && child is T && (child as T).Name.Equals(name))
                    {
                        return (T)child;
                    }
                    else
                    {
                        T childOfChild = FindVisualChildItem<T>(child, name);
                        if (childOfChild != null && childOfChild is T && (childOfChild as T).Name.Equals(name))
                        {
                            return childOfChild;
                        }
                    }
                }
            }
            return null;
        }


        /// <summary>
        /// 获得指定元素的所有子元素(这里需要有一个从DataTemplate里获取控件的函数)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static List<T> GetChildObjects<T>(DependencyObject obj, string name) where T : FrameworkElement
        {
            DependencyObject child = null;
            List<T> childList = new List<T>();
            for (int i = 0; i <= VisualTreeHelper.GetChildrenCount(obj) - 1; i++)
            {
                child = VisualTreeHelper.GetChild(obj, i);
                if (child is T && (((T)child).Name == name || string.IsNullOrEmpty(name)))
                {
                    childList.Add((T)child);
                }
                childList.AddRange(GetChildObjects<T>(child, ""));//指定集合的元素添加到List队尾
            }
            return childList;
        }

        public static T GetParentObject<T>(DependencyObject obj, string name) where T : FrameworkElement
        {
            DependencyObject parent = VisualTreeHelper.GetParent(obj);

            while (parent != null)
            {
                if (parent is T && (((T)parent).Name == name | string.IsNullOrEmpty(name)))
                {
                    return (T)parent;
                }

                parent = VisualTreeHelper.GetParent(parent);
            }

            return null;
        }

        /// <summary>
        /// 将控件转换成图片并保存到指定路径
        /// </summary>
        /// <param name="elem"></param>
        /// <param name="fileName"></param>
        public static BitmapSource GetWindowContent(this FrameworkElement elem)
        {
            if (elem != null)
            {
                RenderTargetBitmap rtb = new RenderTargetBitmap((int)elem.ActualWidth, (int)elem.ActualHeight, 96, 96, PixelFormats.Pbgra32);
                DrawingVisual dv = new DrawingVisual();
                using (DrawingContext dc = dv.RenderOpen())
                {
                    VisualBrush vb = new VisualBrush(elem);
                    dc.DrawRectangle(vb, null, new Rect(new System.Windows.Point(), new System.Windows.Size(elem.ActualWidth, elem.ActualHeight)));
                }

                rtb.Render(dv);

                return rtb;
            }
            return null;
        }
        /// <summary>
        /// 将控件转换成图片并保存到指定路径
        /// </summary>
        /// <param name="elem"></param>
        /// <param name="fileName"></param>
        public static void SaveWindowContent(this FrameworkElement elem, string fileName)
        {
            if (elem != null && !string.IsNullOrEmpty(fileName))
            {
                //FrameworkElement elem = source.Content as FrameworkElement;
                System.Windows.Media.Imaging.RenderTargetBitmap targetBitmap = new System.Windows.Media.Imaging.RenderTargetBitmap(
                                           (int)elem.ActualWidth,
                                           (int)elem.ActualHeight,
                                           96d,
                                           96d,
                                           PixelFormats.Default);

                targetBitmap.Render(elem);
                System.Windows.Media.Imaging.BmpBitmapEncoder encoder = new System.Windows.Media.Imaging.BmpBitmapEncoder();
                encoder.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(targetBitmap));
                System.IO.FileInfo fileInfo = new System.IO.FileInfo(fileName);
                if (!fileInfo.Directory.Exists)
                {
                    fileInfo.Directory.Create();
                }
                // save file to disk
                using (System.IO.FileStream fs = System.IO.File.Open(fileName, System.IO.FileMode.OpenOrCreate))
                {
                    encoder.Save(fs);
                }
            }
        }
    }
}
