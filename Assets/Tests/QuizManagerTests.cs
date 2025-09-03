using NUnit.Framework;
using UnityEngine;
using TMPro;
using System.Reflection; 

public class QuizManagerTests
{
    private QuizManager quizManager;
    private GameObject quizManagerGameObject;

    [SetUp]
    public void PrepararTest()
    {
        quizManagerGameObject = new GameObject();
        quizManager = quizManagerGameObject.AddComponent<QuizManager>();

        quizManager.panels = new GameObject[3];
        quizManager.panels[0] = new GameObject("Panel Pregunta 1");
        quizManager.panels[1] = new GameObject("Panel Pregunta 2");
        quizManager.panels[2] = new GameObject("Panel Resultado");

        quizManager.resultText = new GameObject().AddComponent<TextMeshProUGUI>();
    }

    [Test]
    public void OpcionCorrecta()
    {
        quizManager.OnOptionSelected(true);

        int correctas = quizManager.GetCorrectCountForTest();
        int incorrectas = quizManager.GetWrongCountForTest();
        int indiceActual = quizManager.GetCurrentIndexForTest();

        Assert.AreEqual(1, correctas, "El contador de respuestas correctas debería ser 1.");

        Assert.AreEqual(0, incorrectas, "El contador de respuestas incorrectas no debería haber cambiado.");

        Assert.AreEqual(1, indiceActual, "El índice del panel debería haber avanzado a 1.");

    }
    [Test]
    public void OpcionInCorrecta()
    {
        quizManager.OnOptionSelected(false);

        int correctas = quizManager.GetCorrectCountForTest();
        int incorrectas = quizManager.GetWrongCountForTest();
        int indiceActual = quizManager.GetCurrentIndexForTest();

        Assert.AreEqual(1, incorrectas, "El contador de respuestas incorrectas debería ser 1.");

        Assert.AreEqual(0, correctas, "El contador de respuestas correctas no debería haber cambiado.");

        Assert.AreEqual(1, indiceActual, "El índice del panel debería haber avanzado a 1.");
    }

}