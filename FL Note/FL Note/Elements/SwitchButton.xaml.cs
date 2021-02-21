using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FL_Note.Elements
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SwitchButton : ContentView
    {
        public MyList<string> Item { get; private set; } = new MyList<string>();

        Color _selectedColor = Color.Black;
        public Color SelectedColor
        {
            get
            {
                return _selectedColor;
            }
            set
            {
                _selectedColor = value;
                for (int i = 0; i < buttonGrid.Children.Count; i++)
                {
                    ((Button)buttonGrid.Children[i]).TextColor = i == _selectedIndex ? SelectedTextColor : UnSelectedTextColor;
                    ((Button)buttonGrid.Children[i]).BackgroundColor = i == _selectedIndex ? SelectedColor : UnSelectedColor;
                }
            }
        }

        public Color UnSelectedColor
        {
            get
            {
                return ((ContentView)buttonGrid.Parent).BackgroundColor;
            }
            set
            {
                ((ContentView)buttonGrid.Parent).BackgroundColor = value;
                for (int i = 0; i < buttonGrid.Children.Count; i++)
                {
                    ((Button)buttonGrid.Children[i]).TextColor = i == _selectedIndex ? SelectedTextColor : UnSelectedTextColor;
                    ((Button)buttonGrid.Children[i]).BackgroundColor = i == _selectedIndex ? SelectedColor : UnSelectedColor;
                }
            }
        }

        Color _selectedTextColor = Color.White;
        public Color SelectedTextColor
        {
            get
            {
                return _selectedTextColor;
            }
            set
            {
                _selectedTextColor = value;
                for (int i = 0; i < buttonGrid.Children.Count; i++)
                {
                    ((Button)buttonGrid.Children[i]).TextColor = i == _selectedIndex ? SelectedTextColor : UnSelectedTextColor;
                    ((Button)buttonGrid.Children[i]).BackgroundColor = i == _selectedIndex ? SelectedColor : UnSelectedColor;
                }
            }
        }

        Color _unSelectedTextColor = Color.Black;
        public Color UnSelectedTextColor
        {
            get
            {
                return _unSelectedTextColor;
            }
            set
            {
                _unSelectedTextColor = value;
                for (int i = 0; i < buttonGrid.Children.Count; i++)
                {
                    ((Button)buttonGrid.Children[i]).TextColor = i == _selectedIndex ? SelectedTextColor : UnSelectedTextColor;
                    ((Button)buttonGrid.Children[i]).BackgroundColor = i == _selectedIndex ? SelectedColor : UnSelectedColor;
                }
            }
        }


        byte _selectedIndex = 0;
        public byte SelectedIndex
        {
            get
            {
                return _selectedIndex;
            }
            set
            {
                _selectedIndex = value;
                for (int i = 0; i < buttonGrid.Children.Count; i++)
                {
                    ((Button)buttonGrid.Children[i]).TextColor = i == _selectedIndex ? SelectedTextColor : UnSelectedTextColor;
                    ((Button)buttonGrid.Children[i]).BackgroundColor = i == _selectedIndex ? SelectedColor : UnSelectedColor;
                }
            }
        }

        public SwitchButton ()
		{
            Item.OnAdd += Item_OnAdd;
            Item.OnRemove += Item_OnRemove;
            InitializeComponent();
		}

        private void Item_OnAdd(string obj, int index)
        {
            buttonGrid.ColumnDefinitions.Add(new ColumnDefinition());
            Button button = new Button()
            {
                BackgroundColor = index == SelectedIndex ? SelectedColor : UnSelectedColor,
                Text = obj,
                TextColor = index == SelectedIndex ? SelectedTextColor : UnSelectedTextColor
            };
            button.Clicked += BarButton_Clicked;
            Grid.SetColumn(button, index);
            buttonGrid.Children.Add(button);
        }

        private void BarButton_Clicked(object sender, EventArgs e)
        {
            SelectedIndex = (byte)buttonGrid.Children.IndexOf((View)sender);
        }

        private void Item_OnRemove(string obj, int index)
        {
            buttonGrid.Children.RemoveAt(index);
            buttonGrid.ColumnDefinitions.RemoveAt(index);
        }
    }
}