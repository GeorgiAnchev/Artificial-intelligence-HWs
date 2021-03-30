using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ID3
{
    class TreeNode
    {
        public string NextAttributeName { get; set; }

        public string AttributeValue { get; set; }

        public bool IsLeaf { get; set; }

        public OutcomeClass? Outcome  { get; set; }

        public OutcomeClass DominatingParrentOutcome { get; set; }

        public List<TreeNode> Children { get; set; } = new List<TreeNode>();

    }
}
