using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            try
            {
                // Select some elements in Revit before invoking this command

                // Get the handle of current document.
                UIDocument uidoc = commandData.Application.ActiveUIDocument;

                // Get the element selection of current document.
                Selection selection = uidoc.Selection;
                ICollection<ElementId> selectedIds = uidoc.Selection.GetElementIds();

                if (0 == selectedIds.Count)
                {
                    // If no elements selected.
                    TaskDialog.Show("Revit", "NO SELECCIONASTE NADA");
                }
                else
                {
                    var doc = uidoc.Document;
                    String info = "IDs SELECCIONADOS: ";

                    // Inicia una transacción
                    using (Transaction tx = new Transaction(doc, "Cambiar Nombre Elementos"))
                    {
                        tx.Start();

                        // Itera a través de los elementos seleccionados
                        foreach (ElementId elementId in selectedIds)
                        {
                            Element element = doc.GetElement(elementId);

                            // Cambia el nombre del elemento
                            element.Name = "NuevoNombre " + elementId.IntegerValue; // Reemplaza "NuevoNombre" con el nombre que desees

                            info += "\n\t" + element.Name;
                        }

                        // Comitea la transacción
                        tx.Commit();
                    }

                    TaskDialog.Show("Revit", info);
                }
            }
            catch (Exception e)
            {
                message = e.Message;
                return Autodesk.Revit.UI.Result.Failed;
            }

            return Autodesk.Revit.UI.Result.Succeeded;
        }
    }
}


