using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace XamlBrewer.WinUI3.DataGrid.Sample.Models
{
    public class Mountain : ObservableObject
    {
        private uint _rank;
        private string _name;
        private uint _height;
        private string _range;
        // private Mountain _parent;
        private string _parentMountain;

        [Key]
        public int Id { get; set; }

        public uint Rank
        {
            get => _rank;
            set => SetProperty(ref _rank, value);
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public uint Height
        {
            get => _height;
            set => SetProperty(ref _height, value);
        }

        public string Range
        {
            get => _range;
            set => SetProperty(ref _range, value);
        }

        public string ParentMountain
        {
            get => _parentMountain;
            set => SetProperty(ref _parentMountain, value);
        }
    }
}
