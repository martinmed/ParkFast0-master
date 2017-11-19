using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json.Linq;

using Xamarin.Forms;
using System.Diagnostics;

namespace CustomRenderer
{
    public class SelectMultipleBasePage<T> : ContentPage
    {
        public class WrappedSelection<T> : INotifyPropertyChanged
        {
            public T Item { get; set; }
            bool isSelected = false;
            public bool IsSelected
            {
                get
                {
                    return isSelected;
                }
                set
                {
                    if (isSelected != value)
                    {
                        isSelected = value;
                        PropertyChanged(this, new PropertyChangedEventArgs("IsSelected"));
                        //						PropertyChanged (this, new PropertyChangedEventArgs (nameof (IsSelected))); // C# 6
                    }
                }
            }
            public event PropertyChangedEventHandler PropertyChanged = delegate { };
        }
        public class WrappedItemSelectionTemplate : ViewCell
        {
            
            public WrappedItemSelectionTemplate() : base()
            {
                
                Label name = new Label();
                name.SetBinding(Label.TextProperty, new Binding("Item.Name"));
                Switch mainSwitch = new Switch();
                mainSwitch.SetBinding(Switch.IsToggledProperty, new Binding("IsSelected"));
                RelativeLayout layout = new RelativeLayout();
                layout.Children.Add(name,
                    Constraint.Constant(5),
                    Constraint.Constant(5),
                    Constraint.RelativeToParent(p => p.Width - 60),
                    Constraint.RelativeToParent(p => p.Height - 10)
                );
                layout.Children.Add(mainSwitch,
                    Constraint.RelativeToParent(p => p.Width - 55),
                    Constraint.Constant(5),
                    Constraint.Constant(50),
                    Constraint.RelativeToParent(p => p.Height - 10)
                );

                View = layout;
            }
        }
        public List<WrappedSelection<T>> WrappedItems = new List<WrappedSelection<T>>();
        public SelectMultipleBasePage(List<T> items)
        {
            WrappedItems = items.Select(item => new WrappedSelection<T>() { Item = item, IsSelected = false }).ToList();
            ListView mainList = new ListView()
            {
                ItemsSource = WrappedItems,
                ItemTemplate = new DataTemplate(typeof(WrappedItemSelectionTemplate)),
            };

            mainList.ItemSelected += (sender, e) =>
            {
                if (e.SelectedItem == null) return;
                var o = (WrappedSelection<T>)e.SelectedItem;
                o.IsSelected = !o.IsSelected;
                ((ListView)sender).SelectedItem = null; //de-select
            };
            Content = mainList;

            ToolbarItems.Add(new ToolbarItem("Finalizar", null, Finalizar, ToolbarItemOrder.Primary));           
        }
        void Finalizar()
        {
            var entidades=GetSelection();
            foreach (var v in entidades)
            {
                string name = v.ToString(); 
            }//TODO: insertar a webservice porcada valor que tenga entidades
        }

        private string insertar_usuario(string entidad)
        {
            string datosEntidad;
            string url = "http://tesis2017.000webhostapp.com/webservice.php";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    using (HttpResponseMessage response = client.GetAsync(url).Result)
                    {
                        using (HttpContent content = response.Content)
                        {
                            datosEntidad = content.ReadAsStringAsync().Result;
                            return datosEntidad;
                        }
                    }
                }
            }
            catch
            {
                return "error en la conexion";
            }
        }

        public List<T> GetSelection()
        {
            return WrappedItems.Where(item => item.IsSelected).Select(wrappedItem => wrappedItem.Item).ToList();
        }       
    }
}