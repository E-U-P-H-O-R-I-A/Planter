using UnityEngine;
using UnityEngine.UI;

namespace Utility.UI
{
    [RequireComponent(typeof(RectTransform), typeof(Graphic)), DisallowMultipleComponent]
    [AddComponentMenu("UI/Effects/Extensions/Flippable")]
    public class UIFlippableUtil : MonoBehaviour, IMeshModifier
    {     
        [SerializeField] private bool horizontal = false;
        [SerializeField] private bool vertical = false;
        
        public bool Horizontal
        {
            get => horizontal;
            set => horizontal = value;
        }
        
        public bool Vertical
        {
            get => vertical;
            set => vertical = value;
        }
     
        protected void OnValidate()
        {
            GetComponent<Graphic>().SetVerticesDirty();
        }
     
        public void ModifyMesh(VertexHelper verts)
        {
            RectTransform rectTransform = transform as RectTransform;
         
            for (int i = 0; i < verts.currentVertCount; ++i)
            {
                UIVertex uiVertex = new UIVertex();
                verts.PopulateUIVertex(ref uiVertex,i);
                
                uiVertex.position = new Vector3(
                    (horizontal ? (uiVertex.position.x + (rectTransform.rect.center.x - uiVertex.position.x) * 2) : uiVertex.position.x),
                    (vertical ?  (uiVertex.position.y + (rectTransform.rect.center.y - uiVertex.position.y) * 2) : uiVertex.position.y),
                    uiVertex.position.z
                );
                
                verts.SetUIVertex(uiVertex, i);
            }
        }

        public void ModifyMesh(Mesh mesh)
        {
            //Obsolete member implementation
        }
    }
}