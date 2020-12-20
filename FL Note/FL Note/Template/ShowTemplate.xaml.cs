using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FL_Note
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ShowTemplate : StackLayout
	{
		public ShowTemplate ()
		{
			InitializeComponent ();
		}
	}
}