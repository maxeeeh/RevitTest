using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace RevitTest
{
    [TransactionAttribute(TransactionMode.Manual)]
    public class Class1 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Obtén el documento activo
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            // Obtiene la vista activa
            View activeView = doc.ActiveView;

            // Obtiene todas las categorías en la vista activa
            Categories categories = doc.Settings.Categories;

            // Inicializa una lista para almacenar los elementos de la categoría seleccionada
            List<ElementId> selectedCategoryElementIds = new List<ElementId>();

            string listaCategorias = string.Join(",", categories.Cast<Category>().ToList().Where(c => activeView.GetCategoryHidden(c.Id) == false).Select(ca => ca.Name));
            string cantidad = categories.Cast<Category>().ToList().Count.ToString();

            string categoriasbyElement = "";

            uidoc.Selection.GetElementIds();

            FilteredElementCollector collector = new FilteredElementCollector(doc);

            // Filtramos los elementos por tipo de elemento Family.
            ICollection<Element> familyElements = collector.OfClass(typeof(Family)).ToElements();

            // Creamos una lista para almacenar los nombres de las familias.
            List<string> nombresFamilias = new List<string>();

            // Recorremos la lista de elementos de familia y obtenemos sus nombres.
            foreach (Element familyElement in familyElements)
            {
                Family family = familyElement as Family;
                if (family != null)
                {
                    // Obtenemos el nombre de la familia y lo agregamos a la lista.
                    nombresFamilias.Add(family.Name);
                }
            }

            // Imprimimos los nombres de las familias en la consola.
            foreach (string nombreFamilia in nombresFamilias)
            {
                categoriasbyElement += "\n\t" + nombreFamilia;
            }

            //// Itera a través de las categorías y verifica si están seleccionadas
            //foreach (ElementId elementId in uidoc.Selection.GetElementIds())
            //{
            //    Element element = doc.GetElement(elementId);

            //    categoriasbyElement = "\n\t" + element.Category.Name;
            //}

            if (selectedCategoryElementIds.Count > 0)
            {
                // Los ElementIds de la categoría seleccionada están en selectedCategoryElementIds
                // Puedes usar estos IDs para acceder a los elementos seleccionados de esa categoría
                TaskDialog.Show(cantidad, categoriasbyElement);
            }
            else
            {
                TaskDialog.Show(cantidad, categoriasbyElement);
            }

            return Result.Succeeded;
        }

        //public void asds() 
        //{
        //    UIApplication uiapp = commandData.Application;
        //    //Document doc = uiapp.ActiveUIDocument.Document;
        //    UIDocument uidoc = commandData.Application.ActiveUIDocument;
        //    Document doc = uidoc.Document;

        //    // Obtiene la instancia de la familia seleccionada
        //    ElementId selectedFamilyInstanceId = uidoc.Selection.GetElementIds().FirstOrDefault();

        //    if (selectedFamilyInstanceId != null)
        //    {
        //        Element selectedFamilyInstance = doc.GetElement(selectedFamilyInstanceId);

        //        // Obtén el nombre de la familia seleccionada
        //        string familiaNombre = selectedFamilyInstance.Name;

        //        // Encuentra todos los elementos de la misma familia en el documento
        //        FilteredElementCollector collector = new FilteredElementCollector(doc);
        //        ICollection<ElementId> familyElementIds = collector
        //            .OfClass(typeof(FamilyInstance))
        //            .WhereElementIsNotElementType()
        //            .Where(fi => fi.Name == familiaNombre)
        //            .Select(fi => fi.Id)
        //            .ToList();

        //        // Cambia el nombre de todos los elementos de la misma familia
        //        using (Transaction tx = new Transaction(doc, "Cambiar Nombre de Elementos"))
        //        {
        //            tx.Start();

        //            foreach (ElementId elementId in familyElementIds)
        //            {
        //                Element element = doc.GetElement(elementId);
        //                element.Name = "NuevoNombre"; // Reemplaza "NuevoNombre" con el nombre deseado
        //            }

        //            tx.Commit();
        //        }

        //        TaskDialog.Show("Cambio de Nombre",
        //            "Se cambió el nombre de todos los elementos de la familia seleccionada.");
        //    }
        //    else
        //    {
        //        TaskDialog.Show("Error", "No se ha seleccionado ninguna instancia de familia.");
        //    }
        //}
    }
}


