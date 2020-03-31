using System;
using UnityEngine;
using Verse;
using RimWorld;
using DetailPortraits.Data;

namespace DetailPortraits.Dialog {
    public class Dialog_EditLayerDetail : Window {
		private LayerData layerData;

		private string bufferLockLayerDurationTick = "0";

		private float Height {
			get {
				return 300f;
			}
		}

		public override Vector2 InitialSize {
			get {
				return new Vector2(800f, this.Height);
			}
		}

		public Dialog_EditLayerDetail(LayerData layer) {
			this.layerData = layer;
			this.bufferLockLayerDurationTick = layer.lockLayerDurationTick.ToString();

			this.forcePause = true;
			this.closeOnAccept = false;
			this.closeOnCancel = false;
			this.absorbInputAroundWindow = true;
			this.doCloseButton = true;
			this.doCloseX = true;
			this.optionalTitle = "DetailPortraits.Dialog_EditLayerDetail_Title".Translate(this.layerData.layerName);
		}

		public override void DoWindowContents(Rect inRect) {
			Listing_Standard listing_Standard = new Listing_Standard();
			listing_Standard.ColumnWidth = inRect.width;
			listing_Standard.Begin(inRect);
			DoWindowContentsInternal(listing_Standard);
			listing_Standard.End();
		}

		public void DoWindowContentsInternal(Listing_Standard listing_Standard) {
			Rect rect = listing_Standard.GetRect(Text.LineHeight + 4f);
			Rect rectLabel = new Rect(rect.x, rect.y, 400, rect.height);
			Rect rectTextField = new Rect(rect.x + rectLabel.width, rect.y, 120, rect.height);
			Widgets.Label(rectLabel, "Dialog_EditLayerDetail.LockLayerDurationTick".Translate());
			TooltipHandler.TipRegion(rectLabel, "Dialog_EditLayerDetail.LockLayerDurationTick_Tip".Translate());
			Widgets.TextFieldNumeric(rectTextField, ref this.layerData.lockLayerDurationTick, ref this.bufferLockLayerDurationTick);
		}

	}
}
