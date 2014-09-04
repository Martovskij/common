using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace CommonLibrary
{
  /// <summary>
  /// Хэлпер для TreeView.
  /// </summary>
  public static class TreeViewUtils
  {
    /// <summary>
    /// Выполнить действие над TreeViewItem, соответствующим заданному объекту, отображаемому в дереве.
    /// </summary>
    /// <param name="treeView">Дерево.</param>
    /// <param name="item">Объект, отображаемый в дереве.</param>
    /// <param name="action">Действие над TreeViewItem, соответствующим объекту.</param>
    /// <returns>True, если объект найден и действие выполнено успешно, false - если объект не найден в дереве.</returns>
    public static bool VisitItem(this TreeView treeView, object item, Action<TreeViewItem> action)
    {
      var queue = new Queue<ItemsControl>();
      queue.Enqueue(treeView);
      while (queue.Count > 0)
      {
        var parent = queue.Dequeue();
        TreeViewItem foundItem = parent.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;
        if (foundItem != null)
        {
          action(foundItem);
          return true;
        }
        foreach (var childItem in parent.Items)
        {
          var childItemsControl = parent.ItemContainerGenerator.ContainerFromItem(childItem) as ItemsControl;
          if (childItemsControl != null)
            queue.Enqueue(childItemsControl);
        }
      }
      return false;
    }
  }
}
