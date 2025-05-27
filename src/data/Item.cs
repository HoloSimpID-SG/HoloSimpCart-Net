
using System.Collections.Generic;
using System.Text;

namespace HoloSimpID
{
    public struct Item
    {
        public string itemName;
        public string itemLink;
        public double priceSGD;

        public Item(string itemName, string itemLink = "", double priceSGD = 0)
        {
            this.itemName = itemName;
            this.itemLink = itemLink;
            this.priceSGD = priceSGD;
        }

        public override string ToString() => itemName.Hyperlink(itemLink);

        public static string SafeCreateTypeDB()
        {
            StringBuilder sqlCmdText = new();
            sqlCmdText.AppendLine($@"DO $$");
            sqlCmdText.AppendLine($@"BEGIN");
            // PostgreSQL wants underscored type names, so we use 'item_type' instead of 'ItemType'
            sqlCmdText.AppendLine($@"  IF NOT EXISTS (SELECT 1 FROM pg_type WHERE typname = 'item_type') THEN");
            sqlCmdText.AppendLine($@"    CREATE TYPE item_type AS (");
            sqlCmdText.AppendLine($@"      item_name TEXT,");
            sqlCmdText.AppendLine($@"      item_link TEXT,");
            sqlCmdText.AppendLine($@"      price_sgd DOUBLE PRECISION");
            sqlCmdText.AppendLine($@"    );");
            sqlCmdText.AppendLine($@"  END IF;");
            sqlCmdText.AppendLine($@"END");
            sqlCmdText.AppendLine($@"$$;");
            return sqlCmdText.ToString();
        }
    }
}