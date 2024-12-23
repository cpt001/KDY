using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FactoryFramework
{
    public class Merger : LogisticComponent
    {
        [SerializeField] private int inputIndex = 0; // modulo this number by inputSockets.Length

        private Item _internalItem;
        public override Item OutputItem => _internalItem;

        public override void TransferItems()
        {
            // early bail if bad setup
            if (Inputs.All(o => o == null)) return;
            if (Outputs.All(o => o == null)) return;

            // handle outgoing item
            if (OutputItem != null)
            {
                if (Outputs.Length != 1) throw new System.Exception("Merger can only have one output");
                if (Outputs[0].CanRecieveItem(OutputItem))
                {
                    Item item = OutputItem;
                    TransferItem(Outputs[0]);
                    Outputs[0].RecieveItem(item);
                }
            }
            
            // now try to accept incoming if output was successful
            if (OutputItem != null) return;
            for (int i = 0; i < Inputs.Length; i++)
            {
                var input = Inputs[(i + inputIndex) % Inputs.Length];
                if (input == null) continue;
                if (input.OutputItem == null) continue;
                if (CanRecieveItem(input.OutputItem))
                {
                    Item incoming = input.OutputItem;
                    RecieveItem(incoming);
                    input.TransferItem(this);
                    inputIndex = (inputIndex + i + 1) % Inputs.Length;
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