/*
 * Utente: Dario Varotto
 * Data: 18/08/2008
 * Ora: 2.39
 */

using System;
using System.Collections.Generic;
using HS.Controls.Tree.Base;
using System.Collections;
using System.IO;

namespace nando
{
    /// <summary>
    /// TreeModelFiles keep the datastructure of a filesistem
    /// rooted on a path.
    /// </summary>
    public class PathParseModel : BaseTreeModel
    // TreeModel implements node
    // BaseTreeModel uses get count get children and so on

        //DONE: Change Model type to dynamic to show children only on request
    {
        public List<long> rootId = new List<long>();
        public nandoSql dbConnection;

        public override IEnumerable GetChildren(TreePath treePath)
        {
            //System.Windows.Forms.MessageBox.Show("Getchilds");
            if (this.dbConnection == null) yield break;
            List<long> parentList;

            if (treePath.IsEmpty()) parentList = rootId;
            else
            {
                parentList = new List<long>();
                parentList.Add((treePath.LastNode as FolderItem).SqlId);
            }

            foreach (long parentId in parentList)
            {
                foreach (BaseItem item in dbConnection.getChildren(parentId))
                {
                    yield return item;
                }
            }
        }

        public override int GetChildrenCount(TreePath treePath)
        {
            //System.Windows.Forms.MessageBox.Show("Get count");
            if (treePath.LastNode is FileItem)
                return 0;
            else
                return -1;
        }


    }


}
