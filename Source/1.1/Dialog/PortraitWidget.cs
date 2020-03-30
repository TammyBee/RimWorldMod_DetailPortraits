using DetailPortraits.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using RimWorld;
using Verse;
using Verse.Sound;
using DetailPortraits.Data.DrawingCondition;

namespace DetailPortraits.Dialog {
    [StaticConstructorOnStartup]
    public static class PortraitWidget {
        public static readonly Texture2D ReorderUp = ContentFinder<Texture2D>.Get("UI/Buttons/ReorderUp", true);
        public static readonly Texture2D ReorderDown = ContentFinder<Texture2D>.Get("UI/Buttons/ReorderDown", true);
        public static readonly Texture2D Suspend = ContentFinder<Texture2D>.Get("UI/Buttons/Suspend", true);
        public static readonly Texture2D DeleteX = ContentFinder<Texture2D>.Get("UI/Buttons/Delete", true);
        public static readonly Texture2D Plus = ContentFinder<Texture2D>.Get("UI/Buttons/Plus", true);
        public static readonly Texture2D Edit = ContentFinder<Texture2D>.Get("UI/Buttons/OpenSpecificTab", true);

        public static float LayerPanelHeight = 60f;

        private static string bufferLayerNumber;

        private static FloatField floatFieldPositionX;
        private static FloatField floatFieldPositionY;
        private static FloatField floatFieldScale;
        private static FloatField floatFieldScaleH;

        private static Vector2 scrollPosition;
        private static Vector2 scrollPositionDrawConditions;

        private static string bufferFilePath = "";

        public enum LayerPanelEvent {
            None,
            Select,
            ReorderUp,
            ReorderDown,
            Delete,
            Suspend
        }

        public static LayerPanelEvent DrawLayerPanel(Rect rect,LayerData layerData,ref float y,bool isFront,bool isBack, bool isSelected) {
            LayerPanelEvent lpe = LayerPanelEvent.None;

            Widgets.DrawWindowBackground(rect.ContractedBy(1f));

            Color color = GUI.color;
            if (!layerData.IsAvailable) {
                GUI.color = Color.red;
            }
            string layerPanelDisplayRow1 = "DetailPortraits.Label_LayerPanelDisplayRow1".Translate(layerData.layerNumber,layerData.layerName);
            Color white = Color.white;
            if (layerData.suspended) {
                white = new Color(1f, 0.7f, 0.7f, 0.7f);
            }
            GUI.color = white;
            Widgets.Label(new Rect(rect.x + 32f, rect.y + 6f, rect.width, 24f), layerPanelDisplayRow1);

            y += LayerPanelHeight;

            if (isSelected) {
                Widgets.DrawHighlight(rect.ContractedBy(1f));
            }

            if (!isFront && Widgets.ButtonImage(new Rect(rect.x + 4f, rect.y + 6f, 24f, 24f), PortraitWidget.ReorderUp, white)) {
                SoundDefOf.Tick_High.PlayOneShotOnCamera(null);
                lpe = LayerPanelEvent.ReorderUp;
            }
            if (!isBack && Widgets.ButtonImage(new Rect(rect.x + 4f, rect.y + 30f, 24f, 24f), PortraitWidget.ReorderDown, white)) {
                SoundDefOf.Tick_High.PlayOneShotOnCamera(null);
                lpe = LayerPanelEvent.ReorderDown;
            }

            if (Widgets.ButtonImage(new Rect(rect.x + rect.width - 30f, rect.y + 6f, 24f, 24f), PortraitWidget.DeleteX, white)) {
                SoundDefOf.Click.PlayOneShotOnCamera(null);
                lpe = LayerPanelEvent.Delete;
            }
            if (Widgets.ButtonImage(new Rect(rect.x + rect.width - 54f, rect.y + 6f, 24f, 24f), PortraitWidget.Suspend, white)) {
                SoundDefOf.Click.PlayOneShotOnCamera(null);
                layerData.suspended = !layerData.suspended;
                lpe = LayerPanelEvent.Suspend;
            }
            if (Widgets.ButtonInvisible(rect.ContractedBy(1f))) {
                RefreshLayerEditor(layerData);
                lpe = LayerPanelEvent.Select;
            }

            if (layerData.suspended) {
                Text.Font = GameFont.Medium;
                Text.Anchor = TextAnchor.MiddleCenter;
                Rect rect9 = new Rect(rect.x + rect.width / 2f - 70f, rect.y + rect.height / 2f - 20f, 140f, 40f);
                GUI.DrawTexture(rect9, TexUI.GrayTextBG);
                Widgets.Label(rect9, "SuspendedCaps".Translate());
                Text.Anchor = TextAnchor.UpperLeft;
                Text.Font = GameFont.Small;
            }

            GUI.color = Color.white;
            return lpe;
        }

        public static void DrawLayerEditor(Rect rect, ref LayerData layerData) {
            Widgets.DrawWindowBackground(rect.ExpandedBy(2f));

            TooltipHandler.TipRegion(new Rect(rect.x + 4f, rect.y + 4f, 200f, 24f), "DetailPortraits.Tooltip_Desc_LayerNumber".Translate());
            Widgets.Label(new Rect(rect.x + 4f, rect.y + 4f, 200f, 24f), "DetailPortraits.Label_LayerNumber".Translate());
            Widgets.TextFieldNumeric(new Rect(rect.x + 204f, rect.y + 4f, 120f, 24f), ref layerData.layerNumber,ref bufferLayerNumber);

            TooltipHandler.TipRegion(new Rect(rect.x + 4f, rect.y + 34f, 200f, 24f), "DetailPortraits.Tooltip_Desc_LayerName".Translate());
            Widgets.Label(new Rect(rect.x + 4f, rect.y + 34f, 200f, 24f), "DetailPortraits.Label_LayerName".Translate());
            layerData.layerName = Widgets.TextField(new Rect(rect.x + 204f, rect.y + 32f, 200f, 24f), layerData.layerName);

            TooltipHandler.TipRegion(new Rect(rect.x + 4f, rect.y + 60f, 200f, 24f), "DetailPortraits.Tooltip_Desc_LayerFilePaths".Translate());
            Widgets.Label(new Rect(rect.x + 4f, rect.y + 60f, 200f, 24f), "DetailPortraits.Label_LayerFilePaths".Translate());
            DrawFilePathsEditor(new Rect(rect.x + 204f, rect.y + 60f, 400f, 144f), ref layerData);

            Widgets.Label(new Rect(rect.x + 4f, rect.y + 220f, 200f, 24f), "DetailPortraits.Label_Position".Translate());
            if (floatFieldPositionX.Update(new Rect(rect.x + 204f, rect.y + 220f, 120f, 24f))) {
                if (layerData.localPosition.x != floatFieldPositionX.Value) {
                    layerData.localPosition.x = floatFieldPositionX.Value;
                    layerData.parent.RefreshRenderableLayers();
                }
            }
            if (floatFieldPositionY.Update(new Rect(rect.x + 334f, rect.y + 220f, 120f, 24f))) {
                if (layerData.localPosition.y != floatFieldPositionY.Value) {
                    layerData.localPosition.y = floatFieldPositionY.Value;
                    layerData.parent.RefreshRenderableLayers();
                }
            }

            Widgets.Label(new Rect(rect.x + 4f, rect.y + 250f, 200f, 24f), "DetailPortraits.Label_Scale".Translate());
            if (floatFieldScale.Update(new Rect(rect.x + 204f, rect.y + 250f, 120f, 24f))) {
                if (layerData.localScale != floatFieldScale.Value) {
                    layerData.localScale = floatFieldScale.Value;
                    layerData.parent.RefreshRenderableLayers();
                }
            }
            if (floatFieldScaleH.Update(new Rect(rect.x + 334f, rect.y + 250f, 120f, 24f))) {
                if (layerData.localScaleH != floatFieldScaleH.Value) {
                    layerData.localScaleH = floatFieldScaleH.Value;
                    layerData.parent.RefreshRenderableLayers();
                }
            }

            TooltipHandler.TipRegion(new Rect(rect.x + 4f, rect.y + 280f, 200f, 24f), "DetailPortraits.Tooltip_Desc_DrawingConditions".Translate());
            Widgets.Label(new Rect(rect.x + 4f, rect.y + 280f, 200f, 24f), "DetailPortraits.Label_DrawingConditions".Translate());
            DrawDCEditor(new Rect(rect.x + 208f, rect.y + 280f, 400f, 120f), ref layerData);

            Widgets.Label(new Rect(rect.x + 4f, rect.y + 410f, 200f, 24f), "DetailPortraits.Label_OpenLayerDetail".Translate());
            if(Widgets.ButtonText(new Rect(rect.x + 208f, rect.y + 410f, 50f, 24f), "DetailPortraits.Button_OpenLayerDetail".Translate())) {
                Find.WindowStack.Add(new Dialog_EditLayerDetail(layerData));
            }
        }

        public static void DrawFilePathsEditor(Rect rect, ref LayerData layerData) {
            bufferFilePath = Widgets.TextField(new Rect(rect.x, rect.y, 300f, 24f), bufferFilePath);
            if (Widgets.ButtonImage(new Rect(rect.x + 304f, rect.y, 24f, 24f),PortraitWidget.Plus)) {
                layerData.textureData.texturePaths.Add(bufferFilePath);
                layerData.textureData.RefreshCandidatePaths();
            }

            Rect rectFilePaths = new Rect(rect.x,rect.y + 32f, 320f, rect.height - 28f);
            Rect rectViewFilePaths = new Rect(rect.x, rect.y + 32f, rectFilePaths.width - 16f, layerData.textureData.texturePaths.Count * 28f);
            Widgets.DrawWindowBackground(rectFilePaths.ExpandedBy(2f));
            Widgets.BeginScrollView(rectFilePaths, ref scrollPosition, rectViewFilePaths, true);
            float num = 0f;
            int deleteIndex = -1;
            for (int i = 0; i <layerData.textureData.texturePaths.Count; i++) {
                string path = layerData.textureData.texturePaths[i];
                Rect rectLabel = new Rect(rectFilePaths.x + 4f, rectFilePaths.y + num, rectViewFilePaths.width - 28f, 24f);
                Widgets.Label(rectLabel, path);
                if (Widgets.ButtonInvisible(rectLabel)) {
                    bufferFilePath = path;
                }
                if (Widgets.ButtonImage(new Rect(rectFilePaths.x + rectViewFilePaths.width - 24f, rectFilePaths.y + num, 24f, 24f), PortraitWidget.DeleteX)) {
                    SoundDefOf.Click.PlayOneShotOnCamera(null);
                    deleteIndex = i;
                }
                num += 28f;
            }
            if (deleteIndex != -1) {
                layerData.textureData.texturePaths.RemoveAt(deleteIndex);
                layerData.textureData.RefreshCandidatePaths();
            }
            Widgets.EndScrollView();
        }

        public static void RefreshLayerEditor(LayerData layerData) {
            bufferLayerNumber = layerData.layerNumber.ToString();

            floatFieldPositionX = new FloatField(layerData.localPosition.x);
            floatFieldPositionY = new FloatField(layerData.localPosition.y);
            floatFieldScale = new FloatField(layerData.localScale);
            floatFieldScaleH = new FloatField(layerData.localScaleH);

            scrollPosition = Vector2.zero;
            scrollPositionDrawConditions = Vector2.zero;

            bufferFilePath = "";
        }

        public static void DrawDCEditor(Rect rect, ref LayerData layerData) {
            if (Widgets.ButtonImage(new Rect(rect.x, rect.y, 24f, 24f), PortraitWidget.Plus)) {
                layerData.drawingConditions.Add(new DrawingConditionData());
                Find.WindowStack.Add(new Dialog_EditDrawingCondition(layerData.drawingConditions[layerData.drawingConditions.Count - 1]));
            }

            Rect rectDrawConditions = new Rect(rect.x, rect.y + 32f, 320f, rect.height - 32f);
            Rect rectViewDrawConditions = new Rect(rect.x, rect.y + 28f, rectDrawConditions.width - 16f, layerData.drawingConditions.Count * 28f);
            Widgets.DrawWindowBackground(rectDrawConditions.ExpandedBy(2f));
            Widgets.BeginScrollView(rectDrawConditions, ref scrollPositionDrawConditions, rectViewDrawConditions, true);
            float num = 0f;
            int deleteIndex = -1;
            for (int i = 0; i < layerData.drawingConditions.Count; i++) {
                DrawingConditionData dcd = layerData.drawingConditions[i];
                Color color = GUI.color;
                if (!dcd.IsAvailable) {
                    GUI.color = Color.red;
                }
                Rect rectLabel = new Rect(rectDrawConditions.x + 4f, rectDrawConditions.y + num, rectViewDrawConditions.width - 52f, 22f);
                Widgets.Label(rectLabel, dcd.GetLabel());
                TooltipHandler.TipRegion(rectLabel, dcd.GetLabel());
                GUI.color = color;
                if (Widgets.ButtonImage(new Rect(rectDrawConditions.x + rectViewDrawConditions.width - 48f, rectDrawConditions.y + num, 24f, 24f), PortraitWidget.Edit)) {
                    Find.WindowStack.Add(new Dialog_EditDrawingCondition(layerData.drawingConditions[i]));
                }
                if (Widgets.ButtonImage(new Rect(rectDrawConditions.x + rectViewDrawConditions.width - 24f, rectDrawConditions.y + num, 24f, 24f), PortraitWidget.DeleteX)) {
                    SoundDefOf.Click.PlayOneShotOnCamera(null);
                    deleteIndex = i;
                }
                num += 28f;
            }
            if (deleteIndex != -1) {
                layerData.drawingConditions.RemoveAt(deleteIndex);
            }
            Widgets.EndScrollView();
        }
    }
}
