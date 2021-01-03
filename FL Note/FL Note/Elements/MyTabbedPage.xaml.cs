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
	public partial class MyTabbedPage : ContentView
    {
        public MyList<View> view { get; private set; } = new MyList<View>();

        public List<string> barTabText { get; private set; } = new List<string>();

        public Color SelectedTabColor { get; set; } = Color.Black;

        public Color UnselectedTabColor { get; set; } = Color.Black;

        public Color BarBackgroundColor
        {
            get
            {
                return ((ContentView)buttonGrid.Parent).BackgroundColor;
            }
            set
            {
                ((ContentView)buttonGrid.Parent).BackgroundColor = value;
            }
        }

        public bool IsSwipeEnabled
        {
            get
            {
                return carouselView.IsSwipeEnabled;
            }
            set
            {
                carouselView.IsSwipeEnabled = value;
            }
        }

        public MyTabbedPage ()
		{
            view.OnAdd += View_OnAdd;
            view.OnRemove += View_OnRemove;
			InitializeComponent ();
            carouselView.ItemsSource = view;
            carouselView.Scrolled += CarouselView_Scrolled;
        }

        private void CarouselView_Scrolled(object sender, ItemsViewScrolledEventArgs e)
        {
            for(int i = 0; i < buttonGrid.Children.Count; i++)
            {
                ((Button)buttonGrid.Children[i]).TextColor = i == e.CenterItemIndex ? SelectedTabColor : UnselectedTabColor;
                ((StackLayout)selectGrid.Children[i]).BackgroundColor = i == e.CenterItemIndex ? SelectedTabColor : ((ContentView)buttonGrid.Parent).BackgroundColor;
            }
        }

        private void View_OnAdd(View obj, int index)
        {
            buttonGrid.ColumnDefinitions.Add(new ColumnDefinition());
            Button button = new Button()
            {
                BackgroundColor = ((ContentView)buttonGrid.Parent).BackgroundColor,
                Text = barTabText.Count > index ? barTabText[index] : index.ToString()
            };
            button.Clicked += BarButton_Clicked;
            Grid.SetColumn(button, index);
            buttonGrid.Children.Add(button);

            selectGrid.ColumnDefinitions.Add(new ColumnDefinition());
            StackLayout stackLayout = new StackLayout();
            {
                BackgroundColor = ((ContentView)buttonGrid.Parent).BackgroundColor;
            }
            Grid.SetColumn(stackLayout, index);
            selectGrid.Children.Add(stackLayout);
            carouselView.ItemsSource = view.ToArray();
        }

        private void View_OnRemove(View obj, int index)
        {
            buttonGrid.Children.RemoveAt(index);
            buttonGrid.ColumnDefinitions.RemoveAt(index);
            selectGrid.Children.RemoveAt(index);
            selectGrid.ColumnDefinitions.RemoveAt(index);
            carouselView.ItemsSource = view.ToArray();
        }

        private void BarButton_Clicked(object sender, EventArgs e)
        {
            int index = buttonGrid.Children.IndexOf((View)sender);
            carouselView.ScrollTo(index);
        }
    }
}