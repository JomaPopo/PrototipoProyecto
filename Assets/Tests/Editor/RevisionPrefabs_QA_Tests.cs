using NUnit.Framework;
using UnityEngine;
using UnityEditor;

public class RevisionPrefabs_QA_Tests
{
    [Test]
    public void TodosLosPrefabs_NoDebenTenerScriptsFaltantes()
    {
        // Busca todos los archivos .prefab en la carpeta Assets
        string[] todosLosPaths = AssetDatabase.GetAllAssetPaths();
        int prefabsConErrores = 0;

        foreach (string path in todosLosPaths)
        {
            if (path.EndsWith(".prefab"))
            {
                // Carga el prefab como un GameObject
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab == null) continue;

                // Busca en todos sus componentes (incluyendo hijos)
                var componentes = prefab.GetComponentsInChildren<Component>(true);
                foreach (var c in componentes)
                {
                    if (c == null)
                    {
                        prefabsConErrores++;
                        // Imprime en la consola qué prefab tiene el error
                        Debug.LogError($"[Script Faltante] El prefab en la ruta '{path}' tiene un script roto.", prefab);
                        break; // Salimos de este prefab para no repetir el error
                    }
                }
            }
        }

        // El test falla si CUALQUIER prefab tuvo errores
        Assert.Zero(prefabsConErrores, $"Se encontraron {prefabsConErrores} prefabs con scripts faltantes. Revisa la consola para más detalles.");
    }
}