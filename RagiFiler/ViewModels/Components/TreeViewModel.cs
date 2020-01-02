using Prism.Mvvm;

namespace RagiFiler.ViewModels.Components
{
    class TreeViewModel : BindableBase
    {
        public TreeItemViewModel Root { get; set; }

        public TreeViewModel()
        {
        }
    }
}
