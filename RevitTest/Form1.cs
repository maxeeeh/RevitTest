using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace RevitTest
{
    public partial class Form1 : System.Windows.Forms.Form
    {
        UIDocument uidoc = null;
        Document doc = null;
        ExternalCommandData commandData = null;

        List<TreeNode> SelectedNode = new List<TreeNode>();

        public Form1(Document newDoc, ExternalCommandData newCommandData, UIDocument newUidoc)
        {
            InitializeComponent();
            doc = newDoc;
            uidoc = newUidoc;
            commandData = newCommandData;
            treeView2.CheckBoxes = true;
            treeView2.AfterCheck += treeView2_AfterCheck;

            // Obtiene la vista activa

            //string listaCategorias = string.Join(",", categories.Cast<Category>().ToList().Where(c => activeView.GetCategoryHidden(c.Id) == false).Select(ca => ca.Name));
            //string cantidad = categories.Cast<Category>().ToList().Count.ToString();

            //string categoriasbyElement = "";

            //var element = uidoc.Selection.GetElementIds().First();
            //Element family = doc.GetElement(new ElementId(element.IntegerValue)) as Element;
            LoadTree();

            FilteredElementCollector collector = new FilteredElementCollector(doc);

            //// Filtramos los elementos por tipo de elemento Family.
            ICollection<Element> familyElements = collector.OfClass(typeof(Family)).ToElements();

            //// Creamos una lista para almacenar los nombres de las familias.
            //List<string> nombresFamilias = new List<string>();

            //// Recorremos la lista de elementos de familia y obtenemos sus nombres.
            //foreach (Element familyElement in familyElements)
            //{
            //    Family family = familyElement as Family;
            //    if (family != null)
            //    {
            //        // Obtenemos el nombre de la familia y lo agregamos a la lista.
            //        nombresFamilias.Add(family.Name);
            //    }
            //}

            ////List<string> nombreElementos = new List<string>();
            ////// Obtén los elementos dentro de la familia utilizando la API de Revit.
            ////FilteredElementCollector collectorElemente = new FilteredElementCollector(doc);

            ////// Filtra los elementos por su FamilyInstanceId (que identifica a una instancia específica de la familia).
            ////ICollection<Element> elementosEnFamilia = collector.ToElements();

            ////// Puedes recorrer la lista de elementos dentro de la familia y hacer lo que necesites con ellos.
            ////foreach (Element elemento in elementosEnFamilia)
            ////{
            ////    // Aquí puedes trabajar con los elementos, como obtener sus propiedades o realizar otras operaciones.
            ////    // Por ejemplo, imprimir el nombre del elemento en la consola:
            ////    nombreElementos.Add(elemento.Name);
            ////}

            //// Imprimimos los nombres de las familias en la consola.
            //foreach (string nombreFamilia in nombresFamilias)
            //{
            //    categoriasbyElement += "\n\t" + nombreFamilia;
            //    var elementosDeFamilia = new FilteredElementCollector(doc)
            //                   .OfClass(typeof(FamilyInstance))
            //                   .Where(e => e.Name == nombreFamilia);

            //    if (elementosDeFamilia.Any())
            //    {
            //        foreach (var elemento in elementosDeFamilia)
            //        {
            //            TaskDialog.Show("Elementos de Familia", "Elemento de la familia encontrada: " + nombreFamilia + " --- " + elemento.Name);
            //        }
            //        break;
            //    }
            //}


            ////// Itera a través de las categorías y verifica si están seleccionadas
            ////foreach (ElementId elementId in uidoc.Selection.GetElementIds())
            ////{
            ////    Element element = doc.GetElement(elementId);

            ////    categoriasbyElement = "\n\t" + element.Category.Name;
            ////}

            //if (selectedCategoryElementIds.Count > 0)
            //{
            //    // Los ElementIds de la categoría seleccionada están en selectedCategoryElementIds
            //    // Puedes usar estos IDs para acceder a los elementos seleccionados de esa categoría
            //    TaskDialog.Show(cantidad, categoriasbyElement);
            //}
            //else
            //{
            //    TaskDialog.Show(cantidad, categoriasbyElement);
            //}
        }

        private void SeleccionarDesSeleccionarHijos(TreeNode nodoPadre, bool seleccionar)
        {
            // Seleccionar o deseleccionar el nodo padre
            nodoPadre.Checked = seleccionar;

            if (seleccionar && !SelectedNode.Contains(nodoPadre))
                SelectedNode.Add(nodoPadre);

            if (!seleccionar && SelectedNode.Contains(nodoPadre))
                SelectedNode.Remove(nodoPadre);

            // Recorrer recursivamente los nodos hijos
            foreach (TreeNode nodoHijo in nodoPadre.Nodes)
            {
                SeleccionarDesSeleccionarHijos(nodoHijo, seleccionar);
            }
        }

        private void treeView2_AfterCheck(object sender, TreeViewEventArgs e)
        {

            // Verificar si el evento fue desencadenado por el usuario
            if (e.Action != TreeViewAction.Unknown)
            {
                // Seleccionar o deseleccionar los nodos hijos
                SeleccionarDesSeleccionarHijos(e.Node, e.Node.Checked);
            }
        }

        private void LoadTree()
        {
            // Obtiene todas las categorías en la vista activa
            Categories categories = doc.Settings.Categories;

            treeView2.Nodes.Clear();

            //var element = uidoc.Selection.GetElementIds().First();
            //Element family = doc.GetElement(new ElementId(element.IntegerValue)) as Element;

            foreach (Category category in categories)
            {
                TreeNode categoryNode = new TreeNode(category.Name);

                if (categorySearch.Text != "" && !category.Name.Contains(categorySearch.Text))
                    continue;

                // Obtener las familias dentro de la categoría
                FilteredElementCollector familyCollector = new FilteredElementCollector(doc).
                    WhereElementIsElementType()
                    .OfCategoryId(category.Id);

                FilteredElementCollector elementCollector = new FilteredElementCollector(doc)
                    .OfCategoryId(category.Id);

                List<TreeNode> Familylist = new List<TreeNode>();

                if (familyCollector.Count() == 0 && category.SubCategories.Size == 0)
                    continue;

                bool childrenSubCategories = false;
                foreach (Autodesk.Revit.DB.Category subCategory in category.SubCategories)
                {
                    if (!Familylist.Any(x => x.Text == subCategory.Name))
                    {
                        TreeNode familyNode = new TreeNode(subCategory.Name);

                        if (familySearch.Text != "" && !subCategory.Name.Contains(familySearch.Text))
                            continue;

                        FilteredElementCollector subFamilyCollector = new FilteredElementCollector(doc).
                        WhereElementIsElementType()
                        .OfCategoryId(subCategory.Id);

                        if (subFamilyCollector.Count() == 0)
                            continue;
                        else
                            childrenSubCategories = true;

                        Familylist.Add(familyNode);

                        Families(familyNode, Familylist, subFamilyCollector);

                        categoryNode.Nodes.Add(familyNode);
                    }
                }

                if (familyCollector.Count() == 0  && !childrenSubCategories)
                    continue;

                Families(categoryNode, Familylist, familyCollector);

                if (categoryNode.Nodes.Count > 0)
                    treeView2.Nodes.Add(categoryNode);
            }

        }

        private void Families(TreeNode categoryNode, List<TreeNode> Familylist, FilteredElementCollector familyCollector)
        {

            foreach (ElementType familyType in familyCollector)
            {
                if (!Familylist.Any(x => x.Text == familyType.FamilyName))
                {
                    TreeNode familyNode = new TreeNode(familyType.FamilyName);

                    if (familySearch.Text != "" && !familyType.FamilyName.Contains(familySearch.Text))
                        continue;

                    Familylist.Add(familyNode);
                    categoryNode.Nodes.Add(familyNode);
                }
            }


            foreach (ElementType familyType in familyCollector)
            {
                if (Familylist.Any(x => x.Text == familyType.Name))
                {
                    TreeNode familyNode = Familylist.Find(x => x.Text == familyType.Name);
                    familyNode.Tag = familyType;
                }
            }

            foreach (ElementType familyType in familyCollector)
            {
                TreeNode familyNode = new TreeNode(familyType.Name);

                familyNode.Tag = familyType;

                Familylist.Find(x => x.Text == familyType.FamilyName).Nodes.Add(familyNode);

            }

        }

        public void asds()
        {
            UIApplication uiapp = commandData.Application;
            //Document doc = uiapp.ActiveUIDocument.Document;
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            // Obtiene la instancia de la familia seleccionada
            ElementId selectedFamilyInstanceId = uidoc.Selection.GetElementIds().FirstOrDefault();

            if (selectedFamilyInstanceId != null)
            {
                Element selectedFamilyInstance = doc.GetElement(selectedFamilyInstanceId);

                // Obtén el nombre de la familia seleccionada
                string familiaNombre = selectedFamilyInstance.Name;

                // Encuentra todos los elementos de la misma familia en el documento
                FilteredElementCollector collector = new FilteredElementCollector(doc);
                ICollection<ElementId> familyElementIds = collector
                    .OfClass(typeof(FamilyInstance))
                    .WhereElementIsNotElementType()
                    .Where(fi => fi.Name == familiaNombre)
                    .Select(fi => fi.Id)
                    .ToList();

                // Cambia el nombre de todos los elementos de la misma familia
                using (Transaction tx = new Transaction(doc, "Cambiar Nombre de Elementos"))
                {
                    tx.Start();

                    foreach (ElementId elementId in familyElementIds)
                    {
                        Element element = doc.GetElement(elementId);
                        element.Name = "NuevoNombre"; // Reemplaza "NuevoNombre" con el nombre deseado
                    }

                    tx.Commit();
                }

                TaskDialog.Show("Cambio de Nombre",
                    "Se cambió el nombre de todos los elementos de la familia seleccionada.");
            }
            else
            {
                TaskDialog.Show("Error", "No se ha seleccionado ninguna instancia de familia.");
            }
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            UIApplication uiapp = commandData.Application;
            //Document doc = uiapp.ActiveUIDocument.Document;
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            List<ElementId> elementList = new List<ElementId>();

            foreach (TreeNode node in SelectedNode)
            {
                // Verificar si el nodo está marcado
                if (node.Checked)
                {
                    if (node.Tag != null && typeof(Autodesk.Revit.DB.Family) == node.Tag.GetType())
                        elementList.Add(((Autodesk.Revit.DB.Family)node.Tag).Id);
                    else
                        continue;

                }
            }

            // Cambia el nombre de todos los elementos de la misma familia
            using (Transaction tx = new Transaction(doc, "Cambiar Nombre de Elementos"))
            {
                tx.Start();

                foreach (ElementId elementId in elementList)
                {
                    Element element = doc.GetElement(elementId);
                    element.Name = beforeText.Text + element.Name + afterText.Text; // Reemplaza "NuevoNombre" con el nombre deseado
                }

                tx.Commit();
            }

            //TaskDialog.Show("Cambio de Nombre",
            //    "Se cambió el nombre de todos los elementos de la familia seleccionada.");

        }

        private void categorySearch_TextChanged(object sender, EventArgs e)
        {
            LoadTree();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadTree();
        }
    }
}
