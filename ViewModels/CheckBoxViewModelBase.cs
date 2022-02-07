using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Linq;

namespace Leo.SdlxliffEditor.ViewModels;

public class CheckBoxViewModelBase<T> : ObservableObject where T : CheckBoxViewModelBase<T>
{
    private List<T> children;
    private bool? isChecked;
    private T parent;

    public CheckBoxViewModelBase(string name)
    {
        Name = name;
        isChecked = false;
    }

    public List<T> Children
    {
        get => children;
        protected set => SetParent(value);
    }

    public bool? IsChecked
    {
        get => isChecked;
        set => SetIsChecked(value, true, true, nameof(IsChecked));
    }

    public string Name { get; }

    private void SetIsChecked(bool? value, bool updateChildren, bool updateParent, string propertyName)
    {
        SetProperty(ref isChecked, value, propertyName);

        if (Children != null && updateChildren && value.HasValue)
        {
            foreach (var child in Children)
            {
                child.SetIsChecked(value, true, false, propertyName);
            }
        }

        if (parent != null && updateParent)
        {
            parent.Update();
        }
    }

    private void SetParent(List<T> value)
    {
        children = value;

        foreach (var child in children)
        {
            child.parent = (T)this;
        }
    }

    private void Update()
    {
        var state = Children.First().IsChecked;
        foreach (var child in Children.Skip(1))
        {
            if (state != child.IsChecked)
            {
                state = null;
                break;
            }
        }

        SetIsChecked(state, false, true, nameof(IsChecked));
    }
}
