using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FactoryFramework
{
    public class Splitter : LogisticComponent
    {
        [SerializeField] private int outputIndex = 0; // modulo this number by outputSockets.Length

        private Item _internalItem;
        public override Item OutputItem => _internalItem;

        public override void TransferItems()
        {
            if (OutputItem == null) return;
            if (Outputs.All(o => o == null)) return;
            for (int i = 0; i < Outputs.Length; i++)
            {
                var output = Outputs[(i+outputIndex) % Outputs.Length];
                if (output == null) continue;
                if (output.CanRecieveItem(OutputItem))
                {
                    Item item = OutputItem;
                    TransferItem(output);
                    output.RecieveItem(item);

                    // set new index and return
                    outputIndex = (outputIndex + i + 1) % Outputs.Length;
                    return;
                }
            }
        }

        public override bool TransferItem(LogisticComponent output)
        {
            _internalItem = null;
            return true;
        }
        public override bool CanRecieveItem(Item item)
        {
            return _internalItem == null;
        }
        public override bool RecieveItem(Item item)
        {
            _internalItem = item;
            return true;
        }
    }
}