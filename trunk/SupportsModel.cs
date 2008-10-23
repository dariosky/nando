using System;
using System.Collections.Generic;
using System.Text;
using HS.Controls.Tree.Base;

namespace nando
{
    public class SupportsModel : TreeModel
    {
    }

    public class SupportNode : HS.Controls.Tree.Node
    {
        private int _type = -1;  // indicate the Resource index for the icon, it will be computed once (when it's equal -1)
        public const int TYPE_CD = 1;
        public const int TYPE_CDA= 2;
        public const int TYPE_DVD = 3;
        public const int TYPE_DVDV = 4;
        public const int TYPE_HD = 5;


        public System.Drawing.Image Icon
        {
            // FUTU: Choose the right icon for DVD video
            // FUTU: parse a cd audio with freedb connection
            // Show icons on support depending of it's type: cd/dvd/hd depending on size. Special case could be: cd-audio, dvd-video
            get
            {
                if (_type == -1) _type = this.supposeType();
                switch (_type)
                {
                    case 1:
                        return nando.Properties.Resources.cd.ToBitmap();
                    case 2:
                        return nando.Properties.Resources.cda.ToBitmap();
                    case 3:
                        return nando.Properties.Resources.dvd.ToBitmap();
                    case 4:
                        return nando.Properties.Resources.dvdv.ToBitmap();
                    default:
                        return nando.Properties.Resources.hd.ToBitmap();
                }


            }
        }

        private int supposeType()
        {
            if (Size > 4700000000) return TYPE_HD;
            if (Size > 744003200) return TYPE_DVD;
            return TYPE_CD;
        }

        private string _name;
        public string Name { get { return _name; } set { _name = value; } }

        private long _size;
        public long Size
        {
            get { return _size; }
            set { _size = value; }
        }
        public string HumanizedSize
        {
            get
            {
                return BaseItem.GetHumanizedSize(this._size);
            }
        }

        private long _sqlid = 0;
        public long SqlId
        {
            get { return _sqlid; }
            set { _sqlid = value; }
        }
        public DateTime CreationDate { get; set; }
    }
}
