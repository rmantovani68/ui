#region Using
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#endregion

namespace HmiExample
{
    public class PropertyItem : INotifyPropertyChanged, IEquatable<PropertyItem>
    {
        private string name;
        public string Name
        {
            get { return this.name; }
            set
            {
                if (this.name != value)
                {
                    this.name = value;
                    this.NotifyPropertyChanged("Name");
                }
            }
        }

        private string path;
        public string Path
        {
            get { return this.path; }
            set
            {
                if (this.path != value)
                {
                    this.path = value;
                    this.NotifyPropertyChanged("Path");
                }
            }
        }


        private string type;
        public string Type
        {
            get { return this.type; }
            set
            {
                if (this.type != value)
                {
                    this.type = value;
                    this.NotifyPropertyChanged("Type");
                }
            }
        }

        private object value;
        public object Value
        {
            get { return this.value; }
            set
            {
                if (this.value != value)
                {
                    this.value = value;
                    this.NotifyPropertyChanged("Value");
                }
            }
        }

        public PropertyItem(string name,string path,string type)
        {
            this.name = name;
            this.path = path;
            this.type = type;
        }

        public PropertyItem()
        {
        }

        void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        public override string ToString()
        {
            return string.Format("{0}", name);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public override bool Equals(Object obj)
        {
            return Equals(obj as PropertyItem);
        }

        public bool Equals(PropertyItem prop)
        {
            // If parameter is null return false:
            if (prop == null)
            {
                return false;
            }

            // Return true if either fields match:
            return (Path == prop.Path);
        }
        public override int GetHashCode()
        {
            return (this.Path).GetHashCode();
        }

        public static bool operator ==(PropertyItem p1, PropertyItem p2)
        {
            if (((object)p1) == ((object)p2)) return true;
            if (((object)p1) == null || ((object)p2) == null) return false;

            return p1.Equals(p2);
        }

        public static bool operator !=(PropertyItem p1, PropertyItem p2)
        {
            return !(p1 == p2);
        }

    }
}
