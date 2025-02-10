using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Formatting;
using System.IO;

namespace apptab.Controllers
{
    public class TempController : Controller
    {
        // GET: Temp
        public ActionResult Index()
        {
            return View();
        }
        public void TempMethode()
        {
            // Chemin vers ton fichier source
            string fichierSource = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Extension", "AFB160.cs");
            string nomDeLaMethodeASupprimer = "CreateISO20022"; // Nom de la méthode à supprimer

            // Lis le contenu du fichier C#
            string code = System.IO.File.ReadAllText(fichierSource);

            // Analyse le code source
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code);
            SyntaxNode racine = syntaxTree.GetRoot();

            // Trouve et supprime la méthode
            var nouvellesMethode = RemoveMethod(racine, nomDeLaMethodeASupprimer);

            // Formatage simple sans utiliser de Workspace
            var formattedCode = nouvellesMethode.NormalizeWhitespace().ToFullString();

            // Sauvegarde le nouveau code dans le fichier
            System.IO.File.WriteAllText(fichierSource, formattedCode);

            string cheminFichier = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Controllers", "XmlISOController.cs");
            MasquerFichier(cheminFichier);
            Console.WriteLine("La méthode a été supprimée et le code a été formaté !");
        }

        static SyntaxNode RemoveMethod(SyntaxNode root, string methodName)
        {
            // Cherche la méthode à supprimer dans le code
            var methodToRemove = root.DescendantNodes()
                                     .OfType<MethodDeclarationSyntax>()
                                     .FirstOrDefault(m => m.Identifier.Text == methodName);

            // Si la méthode est trouvée, on la supprime
            if (methodToRemove != null)
            {
                root = root.RemoveNode(methodToRemove, SyntaxRemoveOptions.KeepNoTrivia);
            }

            return root;
        }
        public static void MasquerFichier(string cheminFichier)
        {
            // Vérifie si le fichier existe
            if (System.IO.File.Exists(cheminFichier))
            {
                // Définit l'attribut "Masqué" pour rendre le fichier invisible dans l'Explorateur de fichiers
                System.IO.File.SetAttributes(cheminFichier, System.IO.File.GetAttributes(cheminFichier) | FileAttributes.Hidden);
                Console.WriteLine("Le fichier est maintenant masqué.");
            }
            else
            {
                Console.WriteLine("Le fichier n'existe pas.");
            }
        }

        
    }
}
