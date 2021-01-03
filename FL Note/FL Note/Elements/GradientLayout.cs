using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using Xamarin.Forms;

namespace FL_Note.Elements
{
    public enum GradientColorStackMode
    {
        ToRight,
        ToLeft,
        ToTop,
        ToBottom,
        ToTopLeft,
        ToTopRight,
        ToBottomLeft,
        ToBottomRight
    }

    public class GradientLayout : StackLayout
    {
        string _colorsList = "";
        public string ColorsList
        {
            get
            {
                return _colorsList;
            }
            set
            {
                _colorsList = value;
                this.IsVisible = false;
                this.IsVisible = true;
            }
        }

        public Color[] Colors
        {
            get
            {
                string[] hex = ColorsList.Split(',');
                Color[] colors = new Color[hex.Length];

                for (int i = 0; i < hex.Length; i++)
                {
                    string xx = hex[i].Trim();
                    FieldInfo f_key = typeof(Color).GetField(hex[i].Trim());
                    if (f_key != null)
                    {
                        colors[i] = (Color)f_key.GetValue(null);
                    }
                    else
                    {
                        colors[i] = Color.FromHex(hex[i].Trim());
                    }
                }

                return colors;
            }
        }

        public GradientColorStackMode Mode { get; set; }
    }
}