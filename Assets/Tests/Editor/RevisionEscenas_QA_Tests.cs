using NUnit.Framework;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEditor;

public class RevisionEscenas_QA_Tests
{
    // Este test buscará en todas las escenas añadidas en el Build Settings
    [Test]
    public void TodasLasEscenas_NoDebenTenerScriptsFaltantes()
    {
        int escenasConErrores = 0;

        // Itera sobre cada escena en el Build Settings
        foreach (EditorBuildSettingsScene escena in EditorBuildSettings.scenes)
        {
            if (escena.enabled)
            {
                // Abre la escena para poder analizarla
                EditorSceneManager.OpenScene(escena.path, OpenSceneMode.Single);

                int contadorFaltantes = 0;
                var todosLosObjetos = Object.FindObjectsOfType<GameObject>(true);

                // Revisa cada GameObject en la escena abierta
                foreach (var go in todosLosObjetos)
                {
                    var componentes = go.GetComponents<Component>();
                    foreach (var c in componentes)
                    {
                        if (c == null)
                        {
                            contadorFaltantes++;
                            // Imprime en la consola qué objeto tiene el error
                            Debug.LogError($"[Script Faltante] Objeto '{GetHierarchyPath(go)}' en la escena '{escena.path}' tiene un script roto.", go);
                        }
                    }
                }

                if (contadorFaltantes > 0)
                {
                    escenasConErrores++;
                }
            }
        }

        // El test falla si CUALQUIER escena tuvo errores
        Assert.Zero(escenasConErrores, $"Se encontraron {escenasConErrores} escenas con scripts faltantes. Revisa la consola para más detalles.");
    }

    // Función auxiliar para obtener la ruta completa de un objeto
    private static string GetHierarchyPath(GameObject obj)
    {
        string path = "/" + obj.name;
        while (obj.transform.parent != null)
        {
            obj = obj.transform.parent.gameObject;
            path = "/" + obj.name + path;
        }
        return path;
    }
}