//------------------------------------------------------------------------------
// <auto-generated>
//     這段程式碼是由工具產生的。
//     執行階段版本:4.0.30319.42000
//
//     對這個檔案所做的變更可能會造成錯誤的行為，而且如果重新產生程式碼，
//     變更將會遺失。
// </auto-generated>
//------------------------------------------------------------------------------

[assembly: global::Xamarin.Forms.Xaml.XamlResourceIdAttribute("FL_Note.Elements.DrawLayout.xaml", "Elements/DrawLayout.xaml", typeof(global::FL_Note.Elements.DrawLayout))]

namespace FL_Note.Elements {
    
    
    [global::Xamarin.Forms.Xaml.XamlFilePathAttribute("Elements\\DrawLayout.xaml")]
    public partial class DrawLayout : global::Xamarin.Forms.StackLayout {
        
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Xamarin.Forms.Build.Tasks.XamlG", "2.0.0.0")]
        private global::SkiaSharp.Views.Forms.SKCanvasView backView;
        
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Xamarin.Forms.Build.Tasks.XamlG", "2.0.0.0")]
        private global::SkiaSharp.Views.Forms.SKCanvasView canvasView;
        
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Xamarin.Forms.Build.Tasks.XamlG", "2.0.0.0")]
        private global::FL_Note.Elements.MyImageButton ChooseColorButton;
        
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Xamarin.Forms.Build.Tasks.XamlG", "2.0.0.0")]
        private void InitializeComponent() {
            global::Xamarin.Forms.Xaml.Extensions.LoadFromXaml(this, typeof(DrawLayout));
            backView = global::Xamarin.Forms.NameScopeExtensions.FindByName<global::SkiaSharp.Views.Forms.SKCanvasView>(this, "backView");
            canvasView = global::Xamarin.Forms.NameScopeExtensions.FindByName<global::SkiaSharp.Views.Forms.SKCanvasView>(this, "canvasView");
            ChooseColorButton = global::Xamarin.Forms.NameScopeExtensions.FindByName<global::FL_Note.Elements.MyImageButton>(this, "ChooseColorButton");
        }
    }
}
