﻿using apptab;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Configuration.Provider;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Windows.Forms;

namespace apptab.Extension
{
    public class AFB160
    {
        public AFB CreateTOMPROAFB160(bool devise, string codeJ, SI_USERS user)
        {
            SOFTCONNECTSIIG db = new SOFTCONNECTSIIG();
            SOFTCONNECTOM tom = new SOFTCONNECTOM();
            string texteAFB160 = "";
            OPA_HISTORIQUE historique;
            var Fuser = db.SI_USERS.Where(x => x.LOGIN == user.LOGIN && x.IDPROJET == user.IDPROJET).FirstOrDefault();

            /*********             * 0302       * *******/

            var donneurOrde = (from dordre in db.OPA_DONNEURORDRE
                               where dordre.IDSOCIETE == user.IDPROJET && dordre.APPLICATION == "COMPTA"
                               select dordre).FirstOrDefault();
            /***********************NOM de fichier************************/
            string nom2 = (from nom in tom.RPROJET
                           select nom.NOM2).First();
            //string nom2 = "test test/test.test;";
            nom2 = this.traitementNomFichier(nom2);
            DateTime dateAFB = DateTime.Now;
            string fileName = nom2 + codeJ + recupDate((DateTime)donneurOrde.DATE_PAIEMENT) + "_" + this.formatTime(dateAFB.Hour) + this.formatTime(dateAFB.Minute) + this.formatTime(dateAFB.Second);
            int i = 0;
            int y = 0;
            try
            {
                var incr = (from incrmentation in db.OPA_BASE
                            where incrmentation.IDSOCIETE == user.IDPROJET
                            select incrmentation).First();
                i = (int)incr.INCREMENTATION;
                y = (int)incr.INCRORDREVIR;
            }
            catch (Exception ee) { }
            y++;
            texteAFB160 += "0302";
            texteAFB160 += this.formaterTexte(21, "                        ");
            texteAFB160 += this.formaterDatePaie((DateTime)donneurOrde.DATE_PAIEMENT);
            texteAFB160 += this.formaterTexte(24, donneurOrde.DONNEUR_ORDRE);

            texteAFB160 += this.formaterTexte(6, this.ajouter0(6, y.ToString()));//réference ordre de virement
            texteAFB160 += this.formaterTexte(26, "                    ");
            texteAFB160 += this.formaterTexte(5, donneurOrde.CODE_GUICHET);
            texteAFB160 += this.formaterTexte(11, donneurOrde.NUM_COMPTE);
            texteAFB160 += this.formaterTexte(47, "      ");
            texteAFB160 += this.formaterTexte(5, donneurOrde.CODE_BANQUE);
            texteAFB160 += this.formaterTexte(6, " ");
            texteAFB160 += "\r\n";
            /*********             * 0602       * *******/
            var beneficiaires = (from dordre in db.OPA_REGLEMENT
                                 where dordre.IDSOCIETE == user.IDPROJET && dordre.APPLICATION == "TOMPRO"
                                 select dordre).ToList();

            foreach (var bnfcr in beneficiaires)
            {
                historique = new OPA_HISTORIQUE();
                var jrnl = (from mct in tom.MCOMPTA
                            where mct.NUMENREG == bnfcr.NUM
                            select mct.JL).Single();
                var fact = (from mct in tom.MCOMPTA
                            where mct.NUMENREG == bnfcr.NUM
                            select mct).First();
                i++;
                //MessageBox.Show(fact.MONTANT.ToString());

                if (bnfcr.LIBELLE.Length > 11)
                {
                    texteAFB160 += "0602";
                    texteAFB160 += this.formaterTexte(14, "                      ");
                    texteAFB160 += this.formaterTexte(4, jrnl) + this.ajouter0(8, i.ToString());
                    texteAFB160 += this.formaterTexte(24, bnfcr.BENEFICIAIRE);
                    texteAFB160 += this.formaterTexte(14, bnfcr.BANQUE);
                    texteAFB160 += this.formaterTexte(18, " ");
                    texteAFB160 += this.formaterTexte(5, bnfcr.GUICHET);
                    texteAFB160 += this.formaterTexte(11, bnfcr.RIB);
                    texteAFB160 += this.formaterChiffre(16, fact.MONTANT.ToString());
                    texteAFB160 += this.formaterTexte(11, bnfcr.LIBELLE);
                    texteAFB160 += this.formaterTexte(20, " ");
                    texteAFB160 += this.formaterTexte(5, bnfcr.NUM_ETABLISSEMENT);
                    texteAFB160 += this.formaterTexte(6, " ");
                    texteAFB160 += "\r\n";

                    texteAFB160 += "0702";
                    texteAFB160 += this.formaterTexte(14, "                      ");
                    texteAFB160 += this.formaterTexte(4, jrnl.ToString()) + this.ajouter0(8, i.ToString());
                    texteAFB160 += this.formaterTexte(24, bnfcr.BENEFICIAIRE);
                    texteAFB160 += this.formaterTexte(14, bnfcr.BANQUE);
                    texteAFB160 += this.formaterTexte(18, " ");
                    texteAFB160 += this.formaterTexte(5, bnfcr.GUICHET);
                    texteAFB160 += this.formaterTexte(11, bnfcr.RIB);
                    texteAFB160 += this.formaterChiffre(16, "0000");
                    texteAFB160 += this.formatLibelle0702(bnfcr.LIBELLE);
                    texteAFB160 += this.formaterTexte(20, " ");
                    texteAFB160 += this.formaterTexte(5, bnfcr.NUM_ETABLISSEMENT);
                    texteAFB160 += this.formaterTexte(6, " ");
                    texteAFB160 += "\r\n";
                    historique.NUMENREG = bnfcr.NUM;
                    historique.DATEAFB = DateTime.Now;
                    historique.IDUSER = Fuser.ID;
                    historique.AFB = fileName;
                    historique.IDSOCIETE = user.IDPROJET;
                }
                else
                {
                    texteAFB160 += "0602";
                    texteAFB160 += this.formaterTexte(14, "                      ");
                    texteAFB160 += this.formaterTexte(4, jrnl.ToString()) + this.ajouter0(8, i.ToString());
                    texteAFB160 += this.formaterTexte(24, bnfcr.BENEFICIAIRE);
                    texteAFB160 += this.formaterTexte(14, bnfcr.BANQUE);
                    texteAFB160 += this.formaterTexte(18, " ");
                    texteAFB160 += this.formaterTexte(5, bnfcr.GUICHET);
                    texteAFB160 += this.formaterTexte(11, bnfcr.RIB);
                    texteAFB160 += this.formaterChiffre(16, fact.MONTANT.ToString());
                    texteAFB160 += this.formaterTexte(11, bnfcr.LIBELLE);
                    texteAFB160 += this.formaterTexte(20, " ");
                    texteAFB160 += this.formaterTexte(5, bnfcr.NUM_ETABLISSEMENT);
                    texteAFB160 += this.formaterTexte(6, " ");
                    texteAFB160 += "\r\n";
                    historique.NUMENREG = bnfcr.NUM;
                    historique.DATEAFB = DateTime.Now;
                    historique.IDUSER = Fuser.ID;
                    historique.AFB = fileName;
                    historique.IDSOCIETE = user.IDPROJET;
                }
                db.OPA_HISTORIQUE.Add(historique);
                db.SaveChanges();

                /*****************************CHANGER ETAT FACTURE*****************************/

                var virement = (from vrmt in tom.MCOMPTA
                                where vrmt.NUMENREG == bnfcr.NUM
                                select vrmt).Single();
                virement.EVIREMENT = virement.JL + this.recupDate((DateTime)donneurOrde.DATE_PAIEMENT);
                tom.SaveChanges();
            }
            try
            {
                OPA_BASE pbase = (from pb in db.OPA_BASE
                                  where pb.IDSOCIETE == user.IDPROJET
                                  select pb).First();
                pbase.INCREMENTATION = i;
                pbase.INCRORDREVIR = y;
                db.SaveChanges();
            }
            catch (Exception ee) { }

            /*********             * 0802       *********/
            decimal? montant = 0;

            var nums = (from ecrt in db.OPA_REGLEMENT
                        where ecrt.IDSOCIETE == user.IDPROJET && ecrt.APPLICATION == "TOMPRO"
                        select ecrt).ToList();
            foreach (var num in nums)
            {
                var mont = (from mn in tom.MCOMPTA
                            where mn.NUMENREG == num.NUM
                            select mn).Single();
                if (devise)
                {
                    montant += mont.MONTDEV;
                }
                else
                {
                    montant += mont.MONTANT;
                }
            }

            texteAFB160 += "0802";
            texteAFB160 += this.formaterTexte(98, "                        ");
            texteAFB160 += this.formaterChiffre(16, montant.ToString());
            texteAFB160 += this.formaterTexte(42, " ");
            return new AFB() { Fichier = texteAFB160, Chemin = fileName };
            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "Fichiers txt (*.txt)|*.txt";
                sfd.FileName = fileName;
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(sfd.FileName, texteAFB160, Encoding.Default);
                }
            }
        }
        public AFB CreateTOMPAIEAFB160(bool devise, string codeJ, SI_USERS user)
        {
            SOFTCONNECTSIIG db = new SOFTCONNECTSIIG();
            SOFTCONNECTOM tom = new SOFTCONNECTOM();
            string texteAFB160 = "";
            OPA_HISTORIQUE historique;
            var Fuser = db.SI_USERS.Where(x => x.LOGIN == user.LOGIN && x.IDPROJET == user.IDPROJET).FirstOrDefault();
            /*********             * 0302       * *******/

            var donneurOrde = (from dordre in db.OPA_DONNEURORDRE
                               where dordre.IDSOCIETE == user.IDPROJET && dordre.APPLICATION == "PAIE"
                               select dordre).First();
            /***********************NOM de fichier************************/
            string nom2 = (from nom in tom.RPROJET
                           select nom.NOM2).First();
            //string nom2 = "test test/test.test;";
            nom2 = this.traitementNomFichier(nom2);
            DateTime dateAFB = DateTime.Now;
            string fileName = nom2 + codeJ + recupDate((DateTime)donneurOrde.DATE_PAIEMENT) + "_" + this.formatTime(dateAFB.Hour) + this.formatTime(dateAFB.Minute) + this.formatTime(dateAFB.Second);
            int i = 0;
            int y = 0;
            try
            {
                var incr = (from incrmentation in db.OPA_BASE
                            where incrmentation.IDSOCIETE == user.IDPROJET
                            select incrmentation).First();
                i = (int)incr.INCREMENTATION;
                y = (int)incr.INCRORDREVIR;
            }
            catch (Exception ee) { }
            y++;
            texteAFB160 += "0302";
            texteAFB160 += this.formaterTexte(21, "                        ");
            texteAFB160 += this.formaterDatePaie((DateTime)donneurOrde.DATE_PAIEMENT);
            texteAFB160 += this.formaterTexte(24, donneurOrde.DONNEUR_ORDRE);

            texteAFB160 += this.formaterTexte(6, this.ajouter0(6, y.ToString()));//réference ordre de virement
            texteAFB160 += this.formaterTexte(26, "                    ");
            texteAFB160 += this.formaterTexte(5, donneurOrde.CODE_GUICHET);
            texteAFB160 += this.formaterTexte(11, donneurOrde.NUM_COMPTE);
            texteAFB160 += this.formaterTexte(47, "      ");
            texteAFB160 += this.formaterTexte(5, donneurOrde.CODE_BANQUE);
            texteAFB160 += this.formaterTexte(6, " ");
            texteAFB160 += "\r\n";
            /*********             * 0602       * *******/
            var beneficiaires = (from dordre in db.OPA_REGLEMENT
                                 where dordre.IDSOCIETE == user.IDPROJET && dordre.APPLICATION == "PAIE"
                                 select dordre).ToList();
            decimal monttot = 0;

            foreach (var bnfcr in beneficiaires)
            {
                historique = new OPA_HISTORIQUE();
                /*var jrnl = (from mct in tom.MCOMPTA
                            where mct.NUMENREG == bnfcr.NUM
                            select mct.JL).Single();
                var fact = (from mct in tom.MCOMPTA
                            where mct.NUMENREG == bnfcr.NUM
                            select mct).First();*/
                var mont = (from pre in tom.tpa_preparations
                            join sal in tom.tpa_salaries on pre.matricule equals sal.matricule
                            join ban in tom.tpa_BanqueSalaries on sal.matricule equals ban.matricule
                            where pre.numeroautomatique == bnfcr.NUM && pre.code_constante == "NETP"
                            select new DataListTompaie()
                            {
                                Montant = pre.valeur.Value,
                            }).FirstOrDefault();
                //a faire ici

                var jrnl = "jrnl";
                i++;
                //MessageBox.Show(fact.MONTANT.ToString());

                if (bnfcr.LIBELLE.Length > 31)
                {
                    texteAFB160 += "0602";
                    texteAFB160 += this.formaterTexte(14, "                      ");
                    texteAFB160 += this.formaterTexte(4, jrnl) + this.ajouter0(8, i.ToString());
                    texteAFB160 += this.formaterTexte(24, this.traitementNomFichier(bnfcr.BENEFICIAIRE));
                    texteAFB160 += this.formaterTexte(14, bnfcr.BANQUE);
                    texteAFB160 += this.formaterTexte(18, " ");
                    texteAFB160 += this.formaterTexte(5, bnfcr.GUICHET);
                    texteAFB160 += this.formaterTexte(11, bnfcr.RIB);
                    texteAFB160 += this.formaterChiffre(16, mont.Montant.ToString());
                    //texteAFB160 += "2510";
                    texteAFB160 += this.formaterTexte(31, bnfcr.LIBELLE);
                    //texteAFB160 += this.formaterTexte(11, bnfcr.LIBELLE);
                    //texteAFB160 += this.formaterTexte(20, " ");
                    texteAFB160 += this.formaterTexte(5, bnfcr.NUM_ETABLISSEMENT);
                    texteAFB160 += this.formaterTexte(6, " ");
                    texteAFB160 += "\r\n";

                    texteAFB160 += "0702";
                    texteAFB160 += this.formaterTexte(14, "                      ");
                    texteAFB160 += this.formaterTexte(4, jrnl.ToString()) + this.ajouter0(8, i.ToString());
                    texteAFB160 += this.formaterTexte(24, this.traitementNomFichier(bnfcr.BENEFICIAIRE));
                    texteAFB160 += this.formaterTexte(14, bnfcr.BANQUE);
                    texteAFB160 += this.formaterTexte(18, " ");
                    texteAFB160 += this.formaterTexte(5, bnfcr.GUICHET);
                    texteAFB160 += this.formaterTexte(11, bnfcr.RIB);
                    texteAFB160 += this.formaterChiffre(16, "0000");
                    texteAFB160 += this.formatLibelle0702(bnfcr.LIBELLE);
                    texteAFB160 += this.formaterTexte(20, " ");
                    texteAFB160 += this.formaterTexte(5, bnfcr.NUM_ETABLISSEMENT);
                    texteAFB160 += this.formaterTexte(6, " ");
                    texteAFB160 += "\r\n";
                    historique.NUMENREG = bnfcr.NUM;
                    historique.DATEAFB = DateTime.Now;
                    historique.IDUSER = Fuser.ID;
                    historique.AFB = fileName;
                    historique.IDSOCIETE = user.IDPROJET;
                }
                else
                {
                    texteAFB160 += "0602";
                    texteAFB160 += this.formaterTexte(14, "                      ");
                    texteAFB160 += this.formaterTexte(4, jrnl.ToString()) + this.ajouter0(8, i.ToString());
                    texteAFB160 += this.formaterTexte(24, this.traitementNomFichier(bnfcr.BENEFICIAIRE));
                    texteAFB160 += this.formaterTexte(14, bnfcr.BANQUE);
                    texteAFB160 += this.formaterTexte(18, " ");
                    texteAFB160 += this.formaterTexte(5, bnfcr.GUICHET);
                    texteAFB160 += this.formaterTexte(11, bnfcr.RIB);
                    texteAFB160 += this.formaterChiffre(16, mont.Montant.ToString());
                    //texteAFB160 += "2510";
                    texteAFB160 += this.formaterTexte(31, bnfcr.LIBELLE);
                    //texteAFB160 += this.formaterTexte(11, bnfcr.LIBELLE);
                    //texteAFB160 += this.formaterTexte(20, " ");
                    texteAFB160 += this.formaterTexte(5, bnfcr.NUM_ETABLISSEMENT);
                    texteAFB160 += this.formaterTexte(6, " ");
                    texteAFB160 += "\r\n";
                    historique.NUMENREG = bnfcr.NUM;
                    historique.DATEAFB = DateTime.Now;
                    historique.IDUSER = Fuser.ID;
                    historique.AFB = fileName;
                    historique.IDSOCIETE = user.IDPROJET;
                }
                db.OPA_HISTORIQUE.Add(historique);
                db.SaveChanges();
                monttot += mont.Montant;
                /*****************************CHANGER ETAT FACTURE*****************************/

                /* var virement = (from vrmt in tom.MCOMPTA
                                 where vrmt.NUMENREG == bnfcr.NUM
                                 select vrmt).Single();
                 virement.EVIREMENT = virement.JL + this.recupDate((DateTime)donneurOrde.DATE_PAIEMENT);
                 tom.SaveChanges();*/
            }
            try
            {
                OPA_BASE pbase = (from pb in db.OPA_BASE
                                  where pb.IDSOCIETE == user.IDPROJET
                                  select pb).First();
                pbase.INCREMENTATION = i;
                pbase.INCRORDREVIR = y;
                db.SaveChanges();
            }
            catch (Exception ee) { }

            /*********             * 0802       *********/
            decimal? montant = 0;

            var nums = (from ecrt in db.OPA_REGLEMENT
                        where ecrt.IDSOCIETE == user.IDPROJET && ecrt.APPLICATION == "PAIE"
                        select ecrt).ToList();
            /*foreach (var num in nums)
            {
                //var mont = (from mn in tom.tmp_bulletin
                            //where mn.num == num.NUM
                            //select mn).Single();
                var mont = (from mn in tom.tpa_preparations
                            where mn.numeroautomatique == num.NUM
                            select mn).Single();
                if (devise)
                {
                    //montant += mont.MONTDEV; à voir avec Faramalala
                }
                else
                {
                    montant += mont.valeur;
                }
            }*/
            montant = monttot;
            texteAFB160 += "0802";
            texteAFB160 += this.formaterTexte(98, "                        ");
            texteAFB160 += this.formaterChiffre(16, montant.ToString());
            texteAFB160 += this.formaterTexte(42, " ");
            texteAFB160 += "\r\n";

            return new AFB() { Fichier = texteAFB160, Chemin = fileName };// create file text
                                                                          //using (var sfd = new SaveFileDialog())
                                                                          //{
                                                                          //	sfd.Filter = "Fichiers txt (*.txt)|*.txt";
                                                                          //	sfd.FileName = fileName;
                                                                          //	if (sfd.ShowDialog() == DialogResult.OK)
                                                                          //	{
                                                                          //		File.WriteAllText(sfd.FileName, texteAFB160, Encoding.Default);
                                                                          //	}
                                                                          //}
        }
        public AFB CreateBRAFB160(bool devise, string codeJ, SI_USERS user)
        {

            string s = "";
            SOFTCONNECTSIIG db = new SOFTCONNECTSIIG();
            SI_USERS usr = (from u in db.SI_USERS
                            where u.LOGIN == user.LOGIN
                            select u).FirstOrDefault();
            SI_MAPPAGES dbt = (from d in db.OPA_DATABASE
                               join m in db.SI_MAPPAGES on d.IDMAPPAGE equals m.ID
                               where d.IDUSER == usr.ID
                               select m).FirstOrDefault();

            SOFTCONNECTOM.connex = "Data Source=" + dbt.INSTANCE + ";Initial Catalog=" + dbt.DBASE + ";User ID=" + dbt.CONNEXION + ";Password=" + dbt.CONNEXPWD + ";";
            SOFTCONNECTOM tom = new SOFTCONNECTOM();
            //OPAVITOMATE __db = new OPAVITOMATE();


            //string bds =(from b in db.OPA_DATABASE
            //             where )

            string texteAFB160 = "";
            OPA_HISTORIQUEBR historique;

            /********              0302        ******/

            var donneurOrde = (from dordre in db.OPA_DONNEURORDRE
                               where dordre.IDSOCIETE == user.IDPROJET && dordre.APPLICATION == "BR"
                               select dordre).FirstOrDefault();
            /***********************NOM de fichier************************/
            string nom2 = (from nom in tom.RPROJET
                           select nom.NOM2).FirstOrDefault();
            //string nom2 = "test test/test.test;";
            nom2 = this.traitementNomFichier(nom2);
            DateTime dateAFB = DateTime.Now;
            string fileName = nom2 + codeJ + recupDate((DateTime)donneurOrde.DATE_PAIEMENT) + "_" + this.formatTime(dateAFB.Hour) + this.formatTime(dateAFB.Minute) + this.formatTime(dateAFB.Second);
            int i = 0;
            int y = 0;
            try
            {
                var incr = (from incrmentation in db.OPA_BASE
                            where incrmentation.IDSOCIETE == user.IDPROJET
                            select incrmentation).FirstOrDefault();
                if (incr != null)
                {
                    i = (int)incr.INCREMENTATION;
                    y = (int)incr.INCRORDREVIR;
                }
                else
                {
                    i = 0;
                    y = 0;
                    db.OPA_BASE.Add(new OPA_BASE() { NOMBASE = dbt.DBASE, INCREMENTATION = i, INCRORDREVIR = y, IDSOCIETE = usr.IDPROJET });
                    db.SaveChanges();
                }
            }
            catch (Exception ee) { }
            y++;
            texteAFB160 += "0302";
            texteAFB160 += this.formaterTexte(21, "                        ");
            texteAFB160 += this.formaterDatePaie((DateTime)donneurOrde.DATE_PAIEMENT);
            texteAFB160 += this.formaterTexte(24, donneurOrde.DONNEUR_ORDRE);

            texteAFB160 += this.formaterTexte(6, this.ajouter0(6, y.ToString()));//réference ordre de virement
            texteAFB160 += this.formaterTexte(26, "                    ");
            texteAFB160 += this.formaterTexte(5, donneurOrde.CODE_GUICHET);
            texteAFB160 += this.formaterTexte(11, donneurOrde.NUM_COMPTE);
            texteAFB160 += this.formaterTexte(47, "      ");
            texteAFB160 += this.formaterTexte(5, donneurOrde.CODE_BANQUE);
            texteAFB160 += this.formaterTexte(6, " ");
            texteAFB160 += "\r\n";
            /********              0602        ******/
            var beneficiaires = (from dordre in db.OPA_REGLEMENTBR
                                 where dordre.IDSOCIETE == user.IDPROJET && dordre.APPLICATION == "BR"
                                 select dordre).ToList();

            foreach (var bnfcr in beneficiaires)
            {
                historique = new OPA_HISTORIQUEBR();
                /*var jrnl = (from mct in tom.MCOMPTA
                            where mct.NUMENREG == bnfcr.NUM
                            select mct.JL).Single();
                var fact = (from mct in tom.MCOMPTA
                            where mct.NUMENREG == bnfcr.NUM
                            select mct).First();*/
                var mont = (from bul in tom.MOP
                            where bul.NUMEROOP == bnfcr.NUM
                            select bul).FirstOrDefault();
                var jrnl = "jrnl";
                i++;
                //MessageBox.Show(fact.MONTANT.ToString());

                if (bnfcr.LIBELLE.Length > 11)
                {
                    texteAFB160 += "0602";
                    texteAFB160 += this.formaterTexte(14, "                      ");
                    texteAFB160 += this.formaterTexte(4, jrnl) + this.ajouter0(8, i.ToString());
                    texteAFB160 += this.formaterTexte(24, bnfcr.BENEFICIAIRE);
                    texteAFB160 += this.formaterTexte(14, bnfcr.BANQUE);
                    texteAFB160 += this.formaterTexte(18, " ");
                    texteAFB160 += this.formaterTexte(5, bnfcr.GUICHET);
                    texteAFB160 += this.formaterTexte(11, bnfcr.RIB);
                    texteAFB160 += this.formaterChiffre(16, mont.MONTANTLOC.ToString());
                    texteAFB160 += this.formaterTexte(11, bnfcr.LIBELLE);
                    texteAFB160 += this.formaterTexte(20, " ");
                    texteAFB160 += this.formaterTexte(5, bnfcr.NUM_ETABLISSEMENT);
                    texteAFB160 += this.formaterTexte(6, " ");
                    texteAFB160 += "\r\n";

                    texteAFB160 += "0702";
                    texteAFB160 += this.formaterTexte(14, "                      ");
                    texteAFB160 += this.formaterTexte(4, jrnl.ToString()) + this.ajouter0(8, i.ToString());
                    texteAFB160 += this.formaterTexte(24, bnfcr.BENEFICIAIRE);
                    texteAFB160 += this.formaterTexte(14, bnfcr.BANQUE);
                    texteAFB160 += this.formaterTexte(18, " ");
                    texteAFB160 += this.formaterTexte(5, bnfcr.GUICHET);
                    texteAFB160 += this.formaterTexte(11, bnfcr.RIB);
                    texteAFB160 += this.formaterChiffre(16, "0000");
                    texteAFB160 += this.formatLibelle0702(bnfcr.LIBELLE);
                    texteAFB160 += this.formaterTexte(20, " ");
                    texteAFB160 += this.formaterTexte(5, bnfcr.NUM_ETABLISSEMENT);
                    texteAFB160 += this.formaterTexte(6, " ");
                    texteAFB160 += "\r\n";
                    historique.NUMENREG = bnfcr.NUM;
                    historique.DATEAFB = DateTime.Now;
                    historique.IDUSER = usr.ID;
                    historique.AFB = fileName;
                    historique.IDSOCIETE = user.IDPROJET;
                }
                else
                {
                    texteAFB160 += "0602";
                    texteAFB160 += this.formaterTexte(14, "                      ");
                    texteAFB160 += this.formaterTexte(4, jrnl.ToString()) + this.ajouter0(8, i.ToString());
                    texteAFB160 += this.formaterTexte(24, bnfcr.BENEFICIAIRE);
                    texteAFB160 += this.formaterTexte(14, bnfcr.BANQUE);
                    texteAFB160 += this.formaterTexte(18, " ");
                    texteAFB160 += this.formaterTexte(5, bnfcr.GUICHET);
                    texteAFB160 += this.formaterTexte(11, bnfcr.RIB);
                    texteAFB160 += this.formaterChiffre(16, mont.MONTANTLOC.ToString());
                    texteAFB160 += this.formaterTexte(11, bnfcr.LIBELLE);
                    texteAFB160 += this.formaterTexte(20, " ");
                    texteAFB160 += this.formaterTexte(5, bnfcr.NUM_ETABLISSEMENT);
                    texteAFB160 += this.formaterTexte(6, " ");
                    texteAFB160 += "\r\n";
                    historique.NUMENREG = bnfcr.NUM;
                    historique.DATEAFB = DateTime.Now;
                    historique.IDUSER = usr.ID;
                    historique.AFB = fileName;
                    historique.IDSOCIETE = user.IDPROJET;
                }
                db.OPA_HISTORIQUEBR.Add(historique);
                db.SaveChanges();

                /*****************************CHANGER ETAT FACTURE*****************************/

                /* var virement = (from vrmt in tom.MCOMPTA
                                 where vrmt.NUMENREG == bnfcr.NUM
                                 select vrmt).Single();
                 virement.EVIREMENT = virement.JL + this.recupDate((DateTime)donneurOrde.DATE_PAIEMENT);
                 tom.SaveChanges();*/
            }
            try
            {
                OPA_BASE pbase = (from pb in db.OPA_BASE
                                  where pb.IDSOCIETE == user.IDPROJET
                                  select pb).First();
                pbase.INCREMENTATION = i;
                pbase.INCRORDREVIR = y;
                db.SaveChanges();
            }
            catch (Exception ee) { }

            /********              0802       *********/
            decimal? montant = 0;

            var nums = (from ecrt in db.OPA_REGLEMENTBR
                        where ecrt.IDSOCIETE == user.IDPROJET && ecrt.APPLICATION == "BR"
                        select ecrt).ToList();
            foreach (var num in nums)
            {
                var mont = (from mn in tom.MOP
                            where mn.NUMEROOP == num.NUM
                            select mn).FirstOrDefault();
                if (devise)
                {
                    montant += mont.MONTANTDEV; //à voir avec Faramalala
                }
                else
                {
                    montant += mont.MONTANTLOC;
                }
            }

            texteAFB160 += "0802";
            texteAFB160 += this.formaterTexte(98, "                        ");
            texteAFB160 += this.formaterChiffre(16, montant.ToString());
            texteAFB160 += this.formaterTexte(42, " ");

            return new AFB() { Fichier = texteAFB160, Chemin = fileName };// create file text
        }
        private string traitementNomFichier(string s)
        {
            string result = s;
            result = result.ToLower();
            result = result.Replace('é', 'e');
            result = result.Replace('è', 'e');
            result = result.Replace('ê', 'e');
            result = result.Replace('ë', 'e');
            result = result.Replace('à', 'a');
            result = result.Replace('â', 'a');
            result = result.Replace('ä', 'a');
            result = result.Replace('î', 'i');
            result = result.Replace('ï', 'i');
            result = result.Replace('ù', 'u');
            result = result.Replace('û', 'u');
            result = result.Replace('ü', 'u');
            result = result.Replace('ô', 'o');
            result = result.Replace('ö', 'o');
            result = result.Replace(' ', '_');
            result = result.Replace("'", "_");
            result = result.Replace(';', '_');
            result = result.Replace(',', '_');
            result = result.Replace('.', '_');
            result = result.Replace('/', '_');
            result = result.Replace(':', '_');
            return result.ToUpper();
        }
        public void SaveValideSelectEcriture(List<string> listReg, bool devise, SI_USERS user)
        {
            SOFTCONNECTSIIG db = new SOFTCONNECTSIIG();
            SOFTCONNECTOM tom = new SOFTCONNECTOM();
            //bool test = false;
            List<int> supprLignes = new List<int>();
            foreach (string row in listReg)
            {

                //int num = Convert.ToInt32(row);
                int num = Int32.Parse(row);
                try
                {
                    var ecriture = (from mcpt in tom.MCOMPTA
                                        //join rtier in tom.RTIERS on mcpt.COGEAUXI equals rtier.COGEAUXI
                                    where mcpt.NUMENREG == num
                                    select new
                                    {
                                        NUM = mcpt.NUMENREG,
                                        NORD = mcpt.NORD,
                                        NOPIECE = mcpt.NOPIECE,
                                        COGE = mcpt.COGE,
                                        DATE = mcpt.DATCLE,
                                        MONTANT = mcpt.MONTANT,
                                        MONTDEV = mcpt.MONTDEV,
                                        DEVISE = mcpt.DEVISE,
                                        ACTI = mcpt.ACTI,
                                        POSTE = mcpt.POSTE,
                                        LIBELLE = mcpt.LIBELLE,
                                        CONVENTION = mcpt.CONVENTION,
                                        GEO = mcpt.GEO,
                                        PLAN6 = mcpt.PLAN6,
                                        JL = mcpt.JL,
                                        MARCHE = mcpt.MARCHE,
                                        CODE_J = mcpt.JL,
                                        CATEGORIE = mcpt.CATEGORIE
                                        /*BENEFICIAIRE = rtier.NOM,
                                        BANQUE = rtier.BQNOM,
                                        COMPTE_BANQUE = rtier.RIB1,
                                        DOM1 = rtier.DOM1,
                                        DOM2 = rtier.DOM2*/
                                    }).FirstOrDefault();
                    var beneficiaire = (from bn in tom.MCOMPTA
                                        join rtier in tom.RTIERS on bn.COGEAUXI equals rtier.COGEAUXI
                                        where bn.NORD == ecriture.NORD
                                        select new
                                        {
                                            BENEFICIAIRE = rtier.NOM,
                                            BANQUE = rtier.BQNOM,
                                            COMPTE_BANQUE = rtier.RIB1,
                                            DOM1 = rtier.DOM1,
                                            DOM2 = rtier.DOM2
                                        }
                                        ).FirstOrDefault();


                    #region Sauve REGELEMENT & ANOMALIE

                    if (beneficiaire.COMPTE_BANQUE == null || beneficiaire.BENEFICIAIRE == null)
                    {
                        OPA_ANOMALIE panomalie = new OPA_ANOMALIE();
                        panomalie.NUM = ecriture.NUM;
                        panomalie.IDSOCIETE = user.IDPROJET;
                        panomalie.LIBELLE = ecriture.LIBELLE;

                        try
                        {
                            db.OPA_ANOMALIE.Add(panomalie);
                            db.SaveChanges();
                        }
                        catch (Exception ee) { }
                    }
                    else
                    {
                        OPA_REGLEMENT preg = new OPA_REGLEMENT();
                        preg.NUM = ecriture.NUM;
                        preg.DATE = ecriture.DATE;
                        preg.BENEFICIAIRE = beneficiaire.BENEFICIAIRE;
                        preg.BANQUE = beneficiaire.BANQUE;
                        preg.GUICHET = this.RIB(beneficiaire.COMPTE_BANQUE)[1];
                        preg.RIB = this.RIB(beneficiaire.COMPTE_BANQUE)[2];
                        if (devise)
                        {
                            preg.MONTANT = ecriture.MONTDEV;
                        }
                        else
                        {
                            preg.MONTANT = ecriture.MONTANT;
                        }

                        //preg.LIBELLE = ecriture.LIBELLE;
                        preg.LIBELLE = this.formaterTexte(100, ecriture.LIBELLE);
                        preg.NUM_ETABLISSEMENT = this.RIB(beneficiaire.COMPTE_BANQUE)[0];
                        preg.CODE_J = ecriture.CODE_J;
                        preg.DOM1 = beneficiaire.DOM1;
                        preg.DOM2 = beneficiaire.DOM2;
                        preg.CATEGORIE = ecriture.CATEGORIE;
                        preg.APPLICATION = "TOMPRO";
                        preg.IDSOCIETE = user.IDPROJET;

                        try
                        {
                            db.OPA_REGLEMENT.Add(preg);
                            db.SaveChanges();
                        }
                        catch (Exception eee)
                        {
                        }
                    }

                    #endregion
                    //test = true;
                }
                catch (Exception ex)
                {
                    //test = false;
                }


            }

        }
        public List<DataListTompro> getREGLEMENT(SI_USERS user)
        {
            SOFTCONNECTSIIG db = new SOFTCONNECTSIIG();
            SOFTCONNECTOM tom = new SOFTCONNECTOM();
            /**************Remplissage dataGridFactSelect*********/

            List<OPA_REGLEMENT> numRegs = (from num in db.OPA_REGLEMENT
                                           where num.IDSOCIETE == user.IDPROJET
                                           select num).ToList();

            List<DataListTompro> listEcritureSelect = new List<DataListTompro>();
            foreach (OPA_REGLEMENT num in numRegs)
            {
                var ligneRegs = (from lgnR in tom.MCOMPTA
                                 where lgnR.NUMENREG == num.NUM
                                 select lgnR).FirstOrDefault();
                if (ligneRegs != null)
                {
                    listEcritureSelect.Add(new DataListTompro()
                    {
                        No = ligneRegs.NUMENREG,
                        DateOrdre = this.formaterNORD(ligneRegs.NORD),
                        NoPiece = ligneRegs.NOPIECE,
                        Compte = ligneRegs.COGE,
                        Libelle = num.LIBELLE,
                        Debit = ligneRegs.MONTANT.Value,
                        MontantDevise = (ligneRegs.MONTDEV.Value),
                        Mon = ligneRegs.DEVISE,
                        Rang = ligneRegs.ACTI,
                        Poste = ligneRegs.POSTE,
                        FinancementCategorie = ligneRegs.CONVENTION + " " + ligneRegs.CATEGORIE,
                        Commune = ligneRegs.GEO,
                        Plan6 = ligneRegs.PLAN6,
                        Journal = ligneRegs.JL,
                        Marche = ligneRegs.MARCHE

                    });
                }

            }
            return listEcritureSelect;
        }
        public List<DataListTompaie> getREGLEMENTPaie(SI_USERS user)
        {
            SOFTCONNECTSIIG db = new SOFTCONNECTSIIG();
            SOFTCONNECTOM tom = new SOFTCONNECTOM();
            /**************Remplissage dataGridFactSelect*********/

            List<OPA_REGLEMENT> numRegs = db.OPA_REGLEMENT.Where(x => x.IDSOCIETE == user.IDPROJET && x.APPLICATION == "PAIE").ToList();
            //List<OPA_REGLEMENT> numRegs = (from num in db.OPA_REGLEMENT
            // where num.IDSOCIETE == user.IDSOCIETE
            // select num).ToList();

            List<DataListTompaie> list = new List<DataListTompaie>();
            foreach (OPA_REGLEMENT num in numRegs)
            {
                list.Add((from pre in tom.tpa_preparations
                          join sal in tom.tpa_salaries on pre.matricule equals sal.matricule
                          join ban in tom.tpa_BanqueSalaries on sal.matricule equals ban.matricule
                          where pre.numeroautomatique == num.NUM && pre.code_constante == "NETP"
                          select new DataListTompaie()
                          {
                              No = pre.numeroautomatique,
                              Matricule = sal.matricule,
                              Nom = sal.nom,
                              Mois = pre.mois,
                              Annee = pre.annee,
                              Libelle = "Salaire du mois " + sal.matricule + pre.mois,
                              Montant = pre.valeur.Value,
                              Cin = sal.cin,
                              Banque = ban.Agence,
                              CodeBanque = ban.codeLibelleBanque,
                              Guichet = ban.codeGuichet,
                              CompteBanque = ban.numCompte,
                              CleRIB = ban.cle_RIB
                          }).FirstOrDefault());

            }
            return list;
        }
        public List<DataListTomOP> getREGLEMENTBR(SI_USERS user)
        {
            SOFTCONNECTSIIG db = new SOFTCONNECTSIIG();
            SOFTCONNECTOM tom = new SOFTCONNECTOM();
            /**************Remplissage dataGridFactSelect*********/

            List<OPA_REGLEMENTBR> numRegs = (from num in db.OPA_REGLEMENTBR
                                             where num.IDSOCIETE == user.IDPROJET
                                             select num).ToList();

            List<DataListTomOP> listEcritureSelect = new List<DataListTomOP>();
            foreach (OPA_REGLEMENTBR num in numRegs)
            {
                DataListTomOP ligneRegs = (from mcpt in tom.MOP
                                           where mcpt.NUMEROOP == num.NUM
                                           select new DataListTomOP()
                                           {
                                               No = mcpt.NUMEROOP,
                                               Date = mcpt.DATEFACTURE.Value,
                                               NoPiece = mcpt.NUMEROFACTURE,
                                               Compte = mcpt.COGE,
                                               Libelle = mcpt.LIBELLE,
                                               Montant = mcpt.MONTANTLOC.Value,
                                               MontantDevise = mcpt.MONTANTDEV.Value,
                                               Mon = "",
                                               Rang = mcpt.ACTI,
                                               Poste = mcpt.POSTE,
                                               FinancementCategorie = mcpt.CONVENTION + " " + mcpt.CATEGORIE,
                                               Commune = mcpt.GEO,
                                               Plan6 = mcpt.PLAN6,
                                               Marche = "",
                                               Auxi = mcpt.AUXIFOURNISSEUR,
                                               Status = num.ETAT
                                           }).FirstOrDefault();

                listEcritureSelect.Add(ligneRegs);
            }
            return listEcritureSelect;
        }
        public List<DataListTompro> getListAnomalie(SI_USERS user)
        {
            SOFTCONNECTSIIG db = new SOFTCONNECTSIIG();
            SOFTCONNECTOM tom = new SOFTCONNECTOM();
            /**************Remplissage dtAnomalie*******************/
            List<OPA_ANOMALIE> anomalies = (from panom in db.OPA_ANOMALIE
                                            where panom.IDSOCIETE == user.IDPROJET
                                            select panom).ToList();
            List<DataListTompro> listAnomalie = new List<DataListTompro>();
            foreach (OPA_ANOMALIE anomalie in anomalies)
            {
                var factAnoms = (from fac in tom.MCOMPTA
                                 where fac.NUMENREG == anomalie.NUM
                                 select fac).FirstOrDefault();
                //var libelle = (from breg in db.OPA_REGLEMENT
                //               where breg.NUM == anomalie.NUM
                //               select breg.LIBELLE).Single();

                listAnomalie.Add(new DataListTompro()
                {
                    No = factAnoms.NUMENREG,
                    DateOrdre = this.formaterNORD(factAnoms.NORD),
                    NoPiece = factAnoms.NOPIECE,
                    Compte = factAnoms.COGE,
                    Libelle = factAnoms.LIBELLE,
                    //Libelle = libelle,
                    Debit = factAnoms.MONTANT.Value,
                    MontantDevise = factAnoms.MONTDEV.Value,
                    Mon = factAnoms.DEVISE,
                    Rang = factAnoms.ACTI,
                    Poste = factAnoms.POSTE,
                    FinancementCategorie = factAnoms.CONVENTION + " " + factAnoms.CATEGORIE,
                    Commune = factAnoms.GEO,
                    Plan6 = factAnoms.PLAN6,
                    Journal = factAnoms.JL,
                    Marche = factAnoms.MARCHE
                });

            }
            return listAnomalie;

        }
        public List<DataListTomOP> getListAnomalieBR(SI_USERS user)
        {
            SOFTCONNECTSIIG db = new SOFTCONNECTSIIG();
            SOFTCONNECTOM tom = new SOFTCONNECTOM();
            /**************Remplissage dtAnomalie*******************/
            List<OPA_ANOMALIEBR> anomalies = (from panom in db.OPA_ANOMALIEBR
                                              where panom.IDSOCIETE == user.IDPROJET
                                              select panom).ToList();
            List<DataListTomOP> listAnomalie = new List<DataListTomOP>();
            foreach (OPA_ANOMALIEBR anomalie in anomalies)
            {
                DataListTomOP ligneRegs = (from mcpt in tom.MOP
                                           where mcpt.NUMEROOP == anomalie.NUM
                                           select new DataListTomOP()
                                           {
                                               No = mcpt.NUMEROOP,
                                               Date = mcpt.DATEFACTURE.Value,
                                               NoPiece = mcpt.NUMEROFACTURE,
                                               Compte = mcpt.COGE,
                                               Libelle = mcpt.LIBELLE,
                                               Montant = mcpt.MONTANTLOC.Value,
                                               MontantDevise = mcpt.MONTANTDEV.Value,
                                               Mon = "",
                                               Rang = mcpt.ACTI,
                                               Poste = mcpt.POSTE,
                                               FinancementCategorie = mcpt.CONVENTION + " " + mcpt.CATEGORIE,
                                               Commune = mcpt.GEO,
                                               Plan6 = mcpt.PLAN6,
                                               Marche = "",
                                               Auxi = mcpt.AUXIFOURNISSEUR

                                           }).FirstOrDefault();
                //               where breg.NUM == anomalie.NUM
                //               select breg.LIBELLE).Single();

                listAnomalie.Add(ligneRegs);

            }
            return listAnomalie;

        }
        private List<string> RIB(string compteRIB)
        {
            List<string> compteBQ = new List<string>();
            try
            {
                compteRIB = compteRIB.Replace(" ", "");
                string codebq = compteRIB.Substring(0, 5);
                compteBQ.Add(codebq);
                string codeguichet = compteRIB.Substring(5, 5);
                compteBQ.Add(codeguichet);
                string compte = compteRIB.Substring(10, 11);
                compteBQ.Add(compte);
                string cle = compteRIB.Substring(21, 2);
                compteBQ.Add(cle);
            }
            catch (Exception ex)
            {
                //compteRIB = compteRIB.Replace(" ", "");
                string codebq = null;
                compteBQ.Add(codebq);
                string codeguichet = null;
                compteBQ.Add(codeguichet);
                string compte = null;
                compteBQ.Add(compte);
                string cle = null;
                compteBQ.Add(cle);
            }


            return compteBQ;
        }
        public bool saveDonneurOrdre(SI_USERS user, RJL1 djournal, DateTime dateP)// mila modifiena BR ET COMPTA
        {
            SOFTCONNECTSIIG db = new SOFTCONNECTSIIG();
            SOFTCONNECTOM tom = new SOFTCONNECTOM();
            bool test = false;
            OPA_DONNEURORDRE donordre = new OPA_DONNEURORDRE();
            var tdonneur1 = (from dord in db.OPA_DONNEURORDRE
                             where dord.IDSOCIETE == user.IDPROJET
                             select dord).FirstOrDefault();
            var projet = (from prjt in tom.RPROJET
                          select new
                          {
                              SIGLE = prjt.SIGLE,
                              NOM = prjt.NOM,
                              BASE = prjt.NOM2
                          }).First();
            string dordre = "";
            dordre = projet.SIGLE + projet.NOM;
            try
            {
                tdonneur1.CODE_J = djournal.CODE;
                tdonneur1.DATE_PAIEMENT = dateP;
                tdonneur1.DONNEUR_ORDRE = couperText(24, dordre.Replace(" ", ""));
                tdonneur1.CODE_GUICHET = couperText(5, djournal.GUICHET);
                tdonneur1.NUM_COMPTE = couperText(11, djournal.RIB);
                tdonneur1.CODE_BANQUE = couperText(11, djournal.AGENCE);
                tdonneur1.BASE = projet.BASE;
                tdonneur1.APPLICATION = "COMPTA";
            }
            catch (Exception ex)
            {
                donordre.ID = 1;
                donordre.CODE_J = djournal.CODE;
                donordre.IDSOCIETE = user.IDPROJET;
                donordre.DATE_PAIEMENT = dateP;
                donordre.DONNEUR_ORDRE = couperText(24, dordre);
                donordre.CODE_GUICHET = couperText(5, djournal.GUICHET);
                donordre.NUM_COMPTE = couperText(11, djournal.RIB);
                donordre.CODE_BANQUE = djournal.BANQUE;
                donordre.APPLICATION = "COMPTA";
                db.OPA_DONNEURORDRE.Add(donordre);
            }
            try
            {
                db.SaveChanges();
                test = true;
            }
            catch (Exception ee)
            {
                test = false;
            }

            return test;
        }
        public bool saveDonneurOrdrePaie(SI_USERS user, string journal, string libDOrdre, string codeGichet, string numCompte, string codeBq)
        {
            SOFTCONNECTSIIG db = new SOFTCONNECTSIIG();
            SOFTCONNECTOM tom = new SOFTCONNECTOM();
            bool test = false;
            OPA_DONNEURORDRE donordre = new OPA_DONNEURORDRE();
            var tdonneur1 = (from dord in db.OPA_DONNEURORDRE
                             where dord.IDSOCIETE == user.IDPROJET && dord.APPLICATION == "PAIE"
                             select dord).FirstOrDefault();
            var projet = (from prjt in tom.RPROJET
                          select new
                          {
                              SIGLE = prjt.SIGLE,
                              NOM = prjt.NOM,
                              BASE = prjt.NOM2
                          }).FirstOrDefault();
            /*string dordre = "";
            dordre = projet.SIGLE + projet.NOM;*/
            try
            {
                tdonneur1.CODE_J = journal;
                //tdonneur1.CODE_J = djournal.CODE;
                tdonneur1.DATE_PAIEMENT = DateTime.Now;
                tdonneur1.DONNEUR_ORDRE = couperText(24, libDOrdre.Replace(" ", ""));
                tdonneur1.CODE_GUICHET = codeGichet;
                tdonneur1.NUM_COMPTE = numCompte;
                tdonneur1.CODE_BANQUE = codeBq;
                tdonneur1.BASE = projet.BASE;
                tdonneur1.APPLICATION = "PAIE";
            }
            catch (Exception ex)
            {
                //donordre.ID = 1;
                var id = db.OPA_DONNEURORDRE.Select(x => x.ID).OrderByDescending(x => x).FirstOrDefault();
                donordre.ID = id + 1;
                //donordre.CODE_J = djournal.CODE;
                donordre.CODE_J = journal;
                donordre.IDSOCIETE = user.IDPROJET;
                donordre.DATE_PAIEMENT = DateTime.Now;
                donordre.DONNEUR_ORDRE = couperText(24, libDOrdre);
                donordre.CODE_GUICHET = couperText(5, codeGichet);
                donordre.NUM_COMPTE = couperText(11, numCompte);
                donordre.CODE_BANQUE = couperText(5, codeBq);
                donordre.APPLICATION = "PAIE";
                db.OPA_DONNEURORDRE.Add(donordre);
            }
            try
            {
                db.SaveChanges();
                test = true;
            }
            catch (Exception ee)
            {
                test = false;
            }

            return test;
        }
        public bool saveDonneurOrdreBR(SI_USERS user, RJL1 djournal, DateTime dateP)
        {
            SOFTCONNECTSIIG db = new SOFTCONNECTSIIG();
            SOFTCONNECTOM tom = new SOFTCONNECTOM();
            bool test = true;
            OPA_DONNEURORDRE donordre = new OPA_DONNEURORDRE();
            var tdonneur1 = (from dord in db.OPA_DONNEURORDRE
                             where dord.IDSOCIETE == user.IDPROJET && dord.APPLICATION == "BR"
                             select dord).FirstOrDefault();
            var projet = (from prjt in tom.RPROJET
                          select new
                          {
                              SIGLE = prjt.SIGLE,
                              NOM = prjt.NOM,
                              BASE = prjt.NOM2
                          }).FirstOrDefault();
            string dordre = "";
            dordre = projet.SIGLE + projet.NOM;
            try
            {
                tdonneur1.CODE_J = djournal.CODE;
                tdonneur1.DATE_PAIEMENT = dateP;
                tdonneur1.DONNEUR_ORDRE = couperText(24, dordre.Replace(" ", ""));
                tdonneur1.CODE_GUICHET = couperText(5, djournal.GUICHET);
                tdonneur1.NUM_COMPTE = couperText(11, djournal.RIB);
                var testss = djournal.RIB;
                tdonneur1.CODE_BANQUE = couperText(5, djournal.AGENCE);
                tdonneur1.BASE = couperText(100, projet.BASE);
                tdonneur1.APPLICATION = "BR";
            }
            catch (Exception ex)
            {
                var id = db.OPA_DONNEURORDRE.Select(x => x.ID).OrderByDescending(x => x).FirstOrDefault();
                donordre.ID = id + 1;
                donordre.CODE_J = djournal.CODE;
                donordre.IDSOCIETE = user.IDPROJET;
                donordre.DATE_PAIEMENT = dateP;
                donordre.DONNEUR_ORDRE = couperText(24, dordre);
                donordre.CODE_GUICHET = couperText(5, djournal.GUICHET);
                donordre.NUM_COMPTE = couperText(11, djournal.RIB);
                donordre.CODE_BANQUE = couperText(5, djournal.BANQUE);
                donordre.APPLICATION = "BR";
                db.OPA_DONNEURORDRE.Add(donordre);

            }
            try
            {
                db.SaveChanges();
                test = true;
            }
            catch (Exception ee)
            {
                test = false;
            }

            return test;
        }
        public List<DataListTompro> getListEcritureCompta(string journal, DateTime dateD, DateTime dateF, string compteG, string auxi, string auxi1, DateTime dateP, SI_USERS user)
        {
            SOFTCONNECTSIIG db = new SOFTCONNECTSIIG();
            SOFTCONNECTOM tom = new SOFTCONNECTOM();
            List<DataListTompro> list = new List<DataListTompro>();


            RJL1 djournal = (from jrnl in tom.RJL1
                             where jrnl.CODE == journal
                             select jrnl).Single();


            string nombase = "";


            if (djournal.RIB != null && djournal.RIB != "")
            {
                #region Chargement liste écriture
                if (compteG == "" || compteG == "Tous")
                {
                    if (auxi == "")
                    {
                        var reglements1 = (from mcpt in tom.MCOMPTA
                                           where mcpt.JL == journal && mcpt.DATCLE.Date >= dateD.Date && mcpt.DATCLE.Date <= dateF.Date && mcpt.COGE.StartsWith("4") && mcpt.EVIREMENT == null
                                           group mcpt by mcpt.NORD into mcptgroupe
                                           select new
                                           {
                                               Key = mcptgroupe.Key
                                           }).ToList();
                        foreach (var nord in reglements1)
                        {
                            /*var jl = (from rjl in tom.RJL1
                                      where rjl.CODE == journal
                                      select rjl).Single();*/
                            FCOMPTA validation = (from v in tom.FCOMPTA
                                                  where v.NORD == nord.Key
                                                  select v).FirstOrDefault();
                            if (validation != null)
                            {
                                if (validation.STATUT1USER != null || validation.STATUT2USER != null || validation.STATUT3USER != null || validation.STATUT4USER != null)
                                {
                                    string status = "";
                                    if (validation.STATUT1USER != null)
                                        status = validation.STATUT1USER;/*(from s in tom.RCOMPTATRAIT                                                   
                                                  where s.NUM == validation.STATUT1USER
                                                  select s.)*/
                                    if (validation.STATUT2USER != null)
                                        status = validation.STATUT2USER;
                                    if (validation.STATUT3USER != null)
                                        status = validation.STATUT3USER;
                                    if (validation.STATUT4USER != null)
                                        status = validation.STATUT4USER;
                                    try
                                    {
                                        var reglement = (from mcpt in tom.MCOMPTA
                                                         where mcpt.NORD == nord.Key && mcpt.COGE == djournal.COMPTEASSOCIE && mcpt.EVIREMENT == null
                                                         select mcpt).Single();

                                        if (reglement.S == "D")
                                        {

                                            list.Add(new DataListTompro()
                                            {
                                                No = reglement.NUMENREG,
                                                DateOrdre = formaterNORD(reglement.NORD),
                                                NoPiece = reglement.NOPIECE,
                                                Compte = reglement.COGE,
                                                Libelle = reglement.LIBELLE,
                                                Debit = reglement.MONTANT.Value,
                                                Credit = 0,
                                                MontantDevise = reglement.MONTDEV.Value,
                                                Mon = reglement.DEVISE,
                                                Rang = reglement.ACTI,
                                                Poste = reglement.POSTE,
                                                FinancementCategorie = reglement.CONVENTION + " " + reglement.CATEGORIE,
                                                Commune = reglement.GEO,
                                                Plan6 = reglement.PLAN6,
                                                Journal = reglement.JL,
                                                Marche = reglement.MARCHE,
                                                Statut = status
                                            });
                                        }
                                        else
                                        {
                                            list.Add(new DataListTompro()
                                            {
                                                No = reglement.NUMENREG,
                                                DateOrdre = formaterNORD(reglement.NORD),
                                                NoPiece = reglement.NOPIECE,
                                                Compte = reglement.COGE,
                                                Libelle = reglement.LIBELLE,
                                                Debit = 0,
                                                Credit = reglement.MONTANT.Value,
                                                MontantDevise = reglement.MONTDEV.Value,
                                                Mon = reglement.DEVISE,
                                                Rang = reglement.ACTI,
                                                Poste = reglement.POSTE,
                                                FinancementCategorie = reglement.CONVENTION + " " + reglement.CATEGORIE,
                                                Commune = reglement.GEO,
                                                Plan6 = reglement.PLAN6,
                                                Journal = reglement.JL,
                                                Marche = reglement.MARCHE,
                                                Statut = status
                                            });
                                        }
                                    }
                                    catch (Exception ee) { }
                                }
                            }
                            else
                            {
                                try
                                {
                                    var reglement = (from mcpt in tom.MCOMPTA
                                                     where mcpt.NORD == nord.Key && mcpt.COGE == djournal.COMPTEASSOCIE && mcpt.EVIREMENT == null
                                                     select mcpt).Single();

                                    if (reglement.S == "D")
                                    {

                                        list.Add(new DataListTompro()
                                        {
                                            No = reglement.NUMENREG,
                                            DateOrdre = formaterNORD(reglement.NORD),
                                            NoPiece = reglement.NOPIECE,
                                            Compte = reglement.COGE,
                                            Libelle = reglement.LIBELLE,
                                            Debit = reglement.MONTANT.Value,
                                            Credit = 0,
                                            MontantDevise = reglement.MONTDEV.Value,
                                            Mon = reglement.DEVISE,
                                            Rang = reglement.ACTI,
                                            Poste = reglement.POSTE,
                                            FinancementCategorie = reglement.CONVENTION + " " + reglement.CATEGORIE,
                                            Commune = reglement.GEO,
                                            Plan6 = reglement.PLAN6,
                                            Journal = reglement.JL,
                                            Marche = reglement.MARCHE
                                        });
                                    }
                                    else
                                    {
                                        list.Add(new DataListTompro()
                                        {
                                            No = reglement.NUMENREG,
                                            DateOrdre = formaterNORD(reglement.NORD),
                                            NoPiece = reglement.NOPIECE,
                                            Compte = reglement.COGE,
                                            Libelle = reglement.LIBELLE,
                                            Debit = 0,
                                            Credit = reglement.MONTANT.Value,
                                            MontantDevise = reglement.MONTDEV.Value,
                                            Mon = reglement.DEVISE,
                                            Rang = reglement.ACTI,
                                            Poste = reglement.POSTE,
                                            FinancementCategorie = reglement.CONVENTION + " " + reglement.CATEGORIE,
                                            Commune = reglement.GEO,
                                            Plan6 = reglement.PLAN6,
                                            Journal = reglement.JL,
                                            Marche = reglement.MARCHE
                                        });
                                    }
                                }
                                catch (Exception ee) { }
                            }

                        }
                    }
                    else
                    {
                        var reglements1 = (from mcpt in tom.MCOMPTA
                                           where mcpt.JL == journal && mcpt.DATCLE.Date >= dateD.Date && mcpt.DATCLE.Date <= dateF.Date && mcpt.COGE.StartsWith("4") && mcpt.AUXI == auxi && mcpt.EVIREMENT == null
                                           group mcpt by mcpt.NORD into mcptgroupe
                                           select new
                                           {
                                               Key = mcptgroupe.Key
                                           }).ToList();

                        foreach (var nord in reglements1)
                        {
                            /*var jl = (from rjl in Dct.RJL1
                                      where rjl.CODE == journal
                                      select rjl).Single();*/
                            try
                            {
                                var reglement = (from mcpt in tom.MCOMPTA
                                                 where mcpt.NORD == nord.Key && mcpt.COGE == djournal.COMPTEASSOCIE && mcpt.EVIREMENT == null
                                                 select mcpt).Single();
                                if (reglement.S == "D")
                                {
                                    list.Add(new DataListTompro()
                                    {
                                        No = reglement.NUMENREG,
                                        DateOrdre = this.formaterNORD(reglement.NORD),
                                        NoPiece = reglement.NOPIECE,
                                        Compte = reglement.COGE,
                                        Libelle = reglement.LIBELLE,
                                        Debit = (reglement.MONTANT.Value),
                                        Credit = 0,
                                        MontantDevise = (reglement.MONTDEV.Value),
                                        Mon = reglement.DEVISE,
                                        Rang = reglement.ACTI,
                                        Poste = reglement.POSTE,
                                        FinancementCategorie = reglement.CONVENTION + " " + reglement.CATEGORIE,
                                        Commune = reglement.GEO,
                                        Plan6 = reglement.PLAN6,
                                        Journal = reglement.JL,
                                        Marche = reglement.MARCHE
                                    });
                                }
                                else
                                {
                                    list.Add(new DataListTompro()
                                    {
                                        No = reglement.NUMENREG,
                                        DateOrdre = formaterNORD(reglement.NORD),
                                        NoPiece = reglement.NOPIECE,
                                        Compte = reglement.COGE,
                                        Libelle = reglement.LIBELLE,
                                        Debit = 0,
                                        Credit = (reglement.MONTANT.Value),
                                        MontantDevise = (reglement.MONTDEV.Value),
                                        Mon = reglement.DEVISE,
                                        Rang = reglement.ACTI,
                                        Poste = reglement.POSTE,
                                        FinancementCategorie = reglement.CONVENTION + " " + reglement.CATEGORIE,
                                        Commune = reglement.GEO,
                                        Plan6 = reglement.PLAN6,
                                        Journal = reglement.JL,
                                        Marche = reglement.MARCHE
                                    });
                                }
                            }
                            catch (Exception ee) { }

                        }
                    }
                }
                else
                {
                    if (auxi == "" || auxi == "Tous")
                    {
                        var reglements1 = (from mcpt in tom.MCOMPTA
                                           where mcpt.JL == journal && mcpt.DATCLE >= dateD && mcpt.DATCLE <= dateF && mcpt.COGE == compteG && mcpt.EVIREMENT == null
                                           group mcpt by mcpt.NORD into mcptgroupe
                                           select new
                                           {
                                               Key = mcptgroupe.Key
                                           }).ToList();

                        foreach (var nord in reglements1)
                        {
                            /*var jl = (from rjl in Dct.RJL1
                                      where rjl.CODE == journal
                                      select rjl).Single();*/
                            try
                            {
                                var reglement = (from mcpt in tom.MCOMPTA
                                                 where mcpt.NORD == nord.Key && mcpt.COGE == djournal.COMPTEASSOCIE && mcpt.EVIREMENT == null
                                                 select mcpt).Single();
                                if (reglement.S == "D")
                                {
                                    list.Add(new DataListTompro()
                                    {
                                        No = reglement.NUMENREG,
                                        DateOrdre = this.formaterNORD(reglement.NORD),
                                        NoPiece = reglement.NOPIECE,
                                        Compte = reglement.COGE,
                                        Libelle = reglement.LIBELLE,
                                        Debit = reglement.MONTANT.Value,
                                        Credit = 0,
                                        MontantDevise = reglement.MONTDEV.Value,
                                        Mon = reglement.DEVISE,
                                        Rang = reglement.ACTI,
                                        Poste = reglement.POSTE,
                                        FinancementCategorie = reglement.CONVENTION + " " + reglement.CATEGORIE,
                                        Commune = reglement.GEO,
                                        Plan6 = reglement.PLAN6,
                                        Journal = reglement.JL,
                                        Marche = reglement.MARCHE
                                    });
                                }
                                else
                                {
                                    list.Add(new DataListTompro()
                                    {
                                        No = reglement.NUMENREG,
                                        DateOrdre = this.formaterNORD(reglement.NORD),
                                        NoPiece = reglement.NOPIECE,
                                        Compte = reglement.COGE,
                                        Libelle = reglement.LIBELLE,
                                        Debit = 0,
                                        Credit = reglement.MONTANT.Value,
                                        MontantDevise = reglement.MONTDEV.Value,
                                        Mon = reglement.DEVISE,
                                        Rang = reglement.ACTI,
                                        Poste = reglement.POSTE,
                                        FinancementCategorie = reglement.CONVENTION + " " + reglement.CATEGORIE,
                                        Commune = reglement.GEO,
                                        Plan6 = reglement.PLAN6,
                                        Journal = reglement.JL,
                                        Marche = reglement.MARCHE
                                    });
                                }

                            }
                            catch (Exception ee) { }
                        }
                    }
                    else
                    {
                        var reglements1 = tom.MCOMPTA.Where(mcpt => mcpt.JL == journal && mcpt.DATCLE >= dateD && mcpt.DATCLE <= dateF && mcpt.COGE == compteG && mcpt.EVIREMENT == null)
                            .GroupBy(x => x.NORD).Select(x => new
                            {
                                Key = x.Key
                            }).ToList();

                        //var reglements1 = (from mcpt in tom.MCOMPTA
                        //                                     where mcpt.JL == journal && mcpt.DATCLE.Date >= dateD.Date && mcpt.DATCLE.Date <= dateF.Date && mcpt.COGE == compteG && mcpt.AUXI == auxi && mcpt.EVIREMENT == null
                        //                                     group mcpt by mcpt.NORD into mcptgroupe
                        //                                     select new
                        //                                     {
                        //                                         Key = mcptgroupe.Key
                        //                                     }).ToList();


                        foreach (var nord in reglements1)
                        {
                            /*var jl = (from rjl in Dct.RJL1
                                      where rjl.CODE == journal
                                      select rjl).Single();*/
                            try
                            {
                                var reglement = (from mcpt in tom.MCOMPTA
                                                 where mcpt.NORD == nord.Key && mcpt.COGE == djournal.COMPTEASSOCIE && mcpt.EVIREMENT == null
                                                 select mcpt).Single();
                                if (reglement.S == "D")
                                {
                                    list.Add(new DataListTompro()
                                    {
                                        No = reglement.NUMENREG,
                                        DateOrdre = this.formaterNORD(reglement.NORD),
                                        NoPiece = reglement.NOPIECE,
                                        Compte = reglement.COGE,
                                        Libelle = reglement.LIBELLE,
                                        Debit = reglement.MONTANT.Value,
                                        Credit = 0,
                                        MontantDevise = reglement.MONTDEV.Value,
                                        Mon = reglement.DEVISE,
                                        Rang = reglement.ACTI,
                                        Poste = reglement.POSTE,
                                        FinancementCategorie = reglement.CONVENTION + " " + reglement.CATEGORIE,
                                        Commune = reglement.GEO,
                                        Plan6 = reglement.PLAN6,
                                        Journal = reglement.JL,
                                        Marche = reglement.MARCHE
                                    });
                                }
                                else
                                {
                                    list.Add(new DataListTompro()
                                    {
                                        No = reglement.NUMENREG,
                                        DateOrdre = this.formaterNORD(reglement.NORD),
                                        NoPiece = reglement.NOPIECE,
                                        Compte = reglement.COGE,
                                        Libelle = reglement.LIBELLE,
                                        Debit = 0,
                                        Credit = reglement.MONTANT.Value,
                                        MontantDevise = reglement.MONTDEV.Value,
                                        Mon = reglement.DEVISE,
                                        Rang = reglement.ACTI,
                                        Poste = reglement.POSTE,
                                        FinancementCategorie = reglement.CONVENTION + " " + reglement.CATEGORIE,
                                        Commune = reglement.GEO,
                                        Plan6 = reglement.PLAN6,
                                        Journal = reglement.JL,
                                        Marche = reglement.MARCHE
                                    });
                                }
                            }
                            catch (Exception ee) { }

                        }
                    }

                }
                #endregion
                #region Enregistrement donneur d'ordre
                bool test = saveDonneurOrdre(user, djournal, dateP);
                #endregion
                /*var afficheDOrdre = (from dord in db.OPA_DONNEURORDRE
                                     where dord.IDSOCIETE == user.IDSOCIETE
                                     select dord).First();
                this.tNomDOrdre.Text = afficheDOrdre.DONNEUR_ORDRE;

                this.tRIBdordre.Text = afficheDOrdre.CODE_BANQUE + afficheDOrdre.CODE_GUICHET + afficheDOrdre.NUM_COMPTE;*/
            }
            return list;
        }
        public List<DataListTompaie> getListEcriturePaie(string journal, int mois, int annee, string matriculeD, string matriculeF, DateTime dateP, SI_USERS user)
        {
            SOFTCONNECTSIIG db = new SOFTCONNECTSIIG();
            SOFTCONNECTOM tom = new SOFTCONNECTOM();
            List<DataListTompaie> list = new List<DataListTompaie>();
            RJL1 djournal = (from jrnl in tom.RJL1
                             where jrnl.CODE == journal
                             select jrnl).SingleOrDefault();
            string nombase = "";
            if (djournal.RIB != null && djournal.RIB != "")
            {
                #region Chargement liste écriture
                if (matriculeD == "" || matriculeD == "Tous")
                {
                    /*list = (from bul in tom.tmp_bulletin
                            join sal in tom.tpa_salaries on bul.matricule equals sal.matricule
                            join ban in tom.tpa_BanqueSalaries on sal.matricule equals ban.matricule
                            where bul.mois == mois && bul.annee == annee
                            select new DataListTompaie()
                            {
                                No = bul.num,
                                Matricule = sal.matricule,
                                Nom = sal.nom,
                                Mois = bul.mois.Value,
                                Annee = bul.annee.Value,
                                Libelle = bul.montant + sal.matricule + bul.mois,
                                Montant = bul.montant.Value,
                                Cin = sal.cin,
                                Banque = ban.Agence,
                                CodeBanque = ban.codeLibelleBanque,
                                Guichet = ban.codeGuichet,
                                CompteBanque = ban.numCompte,
                                CleRIB = ban.cle_RIB
                            }).ToList();*/
                    list = (from pre in tom.tpa_preparations
                            join sal in tom.tpa_salaries on pre.matricule equals sal.matricule
                            join ban in tom.tpa_BanqueSalaries on sal.matricule equals ban.matricule
                            where pre.mois == mois && pre.annee == annee && pre.code_constante == "NETP"
                            select new DataListTompaie()
                            {
                                No = pre.numeroautomatique,
                                Matricule = sal.matricule,
                                Nom = sal.nom,
                                Mois = pre.mois,
                                Annee = pre.annee,
                                Libelle = "Salaire du mois " + sal.matricule + pre.mois,
                                Montant = pre.valeur.Value,
                                Cin = sal.cin,
                                Banque = ban.Agence,
                                CodeBanque = ban.codeLibelleBanque,
                                Guichet = ban.codeGuichet,
                                CompteBanque = ban.numCompte,
                                CleRIB = ban.cle_RIB
                            }).ToList();

                }
                else if (matriculeD == matriculeF)
                {
                    /*list = (from bul in tom.tmp_bulletin
                            join sal in tom.tpa_salaries on bul.matricule equals sal.matricule
                            join ban in tom.tpa_BanqueSalaries on sal.matricule equals ban.matricule
                            where bul.mois == mois && bul.annee == annee && sal.matricule == matriculeD
                            select new DataListTompaie()
                            {
                                No = bul.num,
                                Matricule = sal.matricule,
                                Nom = sal.nom,
                                Mois = bul.mois.Value,
                                Annee = bul.annee.Value,
                                Libelle = bul.montant + sal.matricule + bul.mois,
                                Montant = bul.montant.Value,
                                Cin = sal.cin,
                                Banque = ban.Agence,
                                CodeBanque = ban.codeLibelleBanque,
                                Guichet = ban.codeGuichet,
                                CompteBanque = ban.numCompte,
                                CleRIB = ban.cle_RIB
                            }).ToList();*/
                    list = (from pre in tom.tpa_preparations
                            join sal in tom.tpa_salaries on pre.matricule equals sal.matricule
                            join ban in tom.tpa_BanqueSalaries on sal.matricule equals ban.matricule
                            where pre.mois == mois && pre.annee == annee && pre.code_constante == "NETP" && sal.matricule == matriculeD
                            select new DataListTompaie()
                            {
                                No = pre.numeroautomatique,
                                Matricule = sal.matricule,
                                Nom = sal.nom,
                                Mois = pre.mois,
                                Annee = pre.annee,
                                Libelle = "Salaire du mois " + sal.matricule + pre.mois,
                                Montant = pre.valeur.Value,
                                Cin = sal.cin,
                                Banque = ban.Agence,
                                CodeBanque = ban.codeLibelleBanque,
                                Guichet = ban.codeGuichet,
                                CompteBanque = ban.numCompte,
                                CleRIB = ban.cle_RIB
                            }).ToList();
                }
                else
                {
                    /*list = (from bul in tom.tmp_bulletin
                            join sal in tom.tpa_salaries on bul.matricule equals sal.matricule
                            join ban in tom.tpa_BanqueSalaries on sal.matricule equals ban.matricule
                            where bul.mois == mois && bul.annee == annee && sal.matricule.CompareTo(matriculeD) >= 0 && sal.matricule.CompareTo(matriculeF) <= 0
                            select new DataListTompaie()
                            {
                                No = bul.num,
                                Matricule = sal.matricule,
                                Nom = sal.nom,
                                Mois = bul.mois.Value,
                                Annee = bul.annee.Value,
                                Libelle = bul.montant + sal.matricule + bul.mois,
                                Montant = bul.montant.Value,
                                Cin = sal.cin,
                                Banque = ban.Agence,
                                CodeBanque = ban.codeLibelleBanque,
                                Guichet = ban.codeGuichet,
                                CompteBanque = ban.numCompte,
                                CleRIB = ban.cle_RIB
                            }).ToList();*/

                    list = (from pre in tom.tpa_preparations
                            join sal in tom.tpa_salaries on pre.matricule equals sal.matricule
                            join ban in tom.tpa_BanqueSalaries on sal.matricule equals ban.matricule
                            where pre.mois == mois && pre.annee == annee && pre.code_constante == "NETP" && sal.matricule.CompareTo(matriculeD) >= 0 && sal.matricule.CompareTo(matriculeF) <= 0
                            select new DataListTompaie()
                            {
                                No = pre.numeroautomatique,
                                Matricule = sal.matricule,
                                Nom = sal.nom,
                                Mois = pre.mois,
                                Annee = pre.annee,
                                Libelle = "Salaire du mois " + sal.matricule + pre.mois,
                                Montant = pre.valeur.Value,
                                Cin = sal.cin,
                                Banque = ban.Agence,
                                CodeBanque = ban.codeLibelleBanque,
                                Guichet = ban.codeGuichet,
                                CompteBanque = ban.numCompte,
                                CleRIB = ban.cle_RIB
                            }).ToList();
                }
                #endregion
                //#region Enregistrement donneur d'ordre
                //bool test = saveDonneurOrdrePaie(user, djournal, dateP);
                foreach (var item in list)
                {
                    //bool test = saveDonneurOrdrePaie(user, libDOrdre, codeGichet, numCompte, codeBq);
                    bool test = saveDonneurOrdrePaie(user, journal, item.Banque, item.Guichet, item.CompteBanque, item.CodeBanque);
                }


                //#endregion
            }
            return list;
        }
        public List<DataListTomOP> getListEcritureBR(string journal, DateTime dateD, DateTime dateF, bool devise, string compteG, string auxi, string etat, DateTime dateP, SI_USERS user)
        {
            SOFTCONNECTSIIG db = new SOFTCONNECTSIIG();
            SOFTCONNECTOM tom = new SOFTCONNECTOM();
            List<DataListTomOP> list = new List<DataListTomOP>();
            if (etat == "VERIFIES")
                etat = null;
            RJL1 djournal = (from jrnl in tom.RJL1
                             where jrnl.CODE == journal
                             select jrnl).Single();//Miova table FOP
            string nombase = "";
            List<string> DjournalFop = tom.FOP.Where(x => x.JOURNAL == journal && (x.ETAPE1USER == etat || x.ETAPE2USER == etat || x.ETAPE3USER == etat || x.ETAPE4USER == etat || x.ETAPE5USER == etat || x.ETAPE6USER == etat || x.ETAPE7USER == etat || x.ETAPE8USER == etat || x.ETAPE9USER == etat || x.ETAPE10USER == etat)).Select(x => x.NUMEROOP).ToList();

            List<MOP> lNoOPS = new List<MOP>();
            foreach (string num in DjournalFop)
            {
                lNoOPS.AddRange((from mo in tom.MOP
                                 where mo.DATEFACTURE >= dateD.Date && mo.DATEFACTURE <= dateF.Date && mo.NUMEROOP == num
                                 select mo).ToList());
            }

            if (djournal.RIB != null && djournal.RIB != "")
            {
                #region Chargement liste écriture
                if (compteG == "" || compteG == "Tous")
                {
                    if (auxi == "")
                    {
                        List<MOP> lNoOp = lNoOPS;
                        foreach (var nord in lNoOp)
                        {
                            try
                            {
                                var reglement = (from mcpt in tom.MOP
                                                 where mcpt.NUMEROOP == nord.NUMEROOP && mcpt.COGE == djournal.COMPTEASSOCIE
                                                 select mcpt).Single();
                                list.Add(new DataListTomOP()
                                {
                                    No = reglement.NUMEROOP,
                                    Date = reglement.DATEFACTURE.Value,
                                    NoPiece = reglement.NUMEROFACTURE,
                                    Compte = reglement.COGE,
                                    Libelle = reglement.LIBELLE,
                                    Montant = reglement.MONTANTLOC.Value,
                                    MontantDevise = reglement.MONTANTDEV.Value,
                                    Mon = "",
                                    Rang = reglement.ACTI,
                                    Poste = reglement.POSTE,
                                    FinancementCategorie = reglement.CONVENTION + " " + reglement.CATEGORIE,
                                    Commune = reglement.GEO,
                                    Plan6 = reglement.PLAN6,
                                    Journal = journal,
                                    Marche = "",
                                    Status = etat
                                });

                            }
                            catch (Exception ee) { }


                        }
                    }
                    else
                    {
                        List<MOP> lNoOp = (from m in lNoOPS
                                           where m.COGEFOURNISSEUR == auxi
                                           select m).ToList();
                        //List<MOP> lNoOp = (from m in lNoOPS
                        //                   where m.AUXIFOURNISSEUR == auxi
                        //                   select m).ToList();
                        foreach (var nord in lNoOp)
                        {
                            try
                            {
                                var reglement = (from mcpt in tom.MOP
                                                 where mcpt.NUMEROOP == nord.NUMEROOP && mcpt.COGE == djournal.COMPTEASSOCIE
                                                 select mcpt).Single();
                                list.Add(new DataListTomOP()
                                {
                                    No = reglement.NUMEROOP,
                                    Date = reglement.DATEFACTURE.Value,
                                    NoPiece = reglement.NUMEROFACTURE,
                                    Compte = reglement.COGE,
                                    Libelle = reglement.LIBELLE,
                                    Montant = reglement.MONTANTLOC.Value,
                                    MontantDevise = reglement.MONTANTDEV.Value,
                                    Mon = "",
                                    Rang = reglement.ACTI,
                                    Poste = reglement.POSTE,
                                    FinancementCategorie = reglement.CONVENTION + " " + reglement.CATEGORIE,
                                    Commune = reglement.GEO,
                                    Plan6 = reglement.PLAN6,
                                    Journal = journal,
                                    Marche = "",
                                    Status = etat
                                });

                            }
                            catch (Exception ee) { }
                        }
                    }
                }
                else
                {
                    List<MOP> lOPCOGE = (from m in lNoOPS
                                         where m.COGEFOURNISSEUR == compteG
                                         select m).ToList();
                    if (auxi == "")
                    {

                        foreach (var nord in lOPCOGE)
                        {
                            try
                            {
                                var reglement = (from mcpt in tom.MOP
                                                 where mcpt.NUMEROOP == nord.NUMEROOP && mcpt.COGE == djournal.COMPTEASSOCIE
                                                 select mcpt).Single();
                                list.Add(new DataListTomOP()
                                {
                                    No = reglement.NUMEROOP,
                                    Date = reglement.DATEFACTURE.Value,
                                    NoPiece = reglement.NUMEROFACTURE,
                                    Compte = reglement.COGE,
                                    Libelle = reglement.LIBELLE,
                                    Montant = reglement.MONTANTLOC.Value,
                                    MontantDevise = reglement.MONTANTDEV.Value,
                                    Mon = "",
                                    Rang = reglement.ACTI,
                                    Poste = reglement.POSTE,
                                    FinancementCategorie = reglement.CONVENTION + " " + reglement.CATEGORIE,
                                    Commune = reglement.GEO,
                                    Plan6 = reglement.PLAN6,
                                    Journal = journal,
                                    Marche = "",
                                    Status = etat
                                });

                            }
                            catch (Exception ee) { }


                        }
                    }
                    else
                    {
                        List<MOP> lNoOp = (from m in lOPCOGE
                                           where m.AUXIFOURNISSEUR == auxi
                                           select m).ToList();
                        foreach (var nord in lNoOp)
                        {
                            try
                            {
                                var reglement = (from mcpt in tom.MOP
                                                 where mcpt.NUMEROOP == nord.NUMEROOP && mcpt.COGE == djournal.COMPTEASSOCIE
                                                 select mcpt).SingleOrDefault();
                                //246610 246340  BR N°00022/01 246610
                                //var reglement = (from mcpt in tom.MOP
                                //                 where mcpt.NUMEROOP == nord.NUMEROOP && mcpt.COGE == djournal.COMPTEASSOCIE
                                //                 select mcpt).Single();
                                if (reglement != null)
                                {
                                    list.Add(new DataListTomOP()
                                    {
                                        No = reglement.NUMEROOP,
                                        Date = reglement.DATEFACTURE.Value,
                                        NoPiece = reglement.NUMEROFACTURE,
                                        Compte = reglement.COGE,
                                        Libelle = reglement.LIBELLE,
                                        Montant = reglement.MONTANTLOC.Value,
                                        MontantDevise = reglement.MONTANTDEV.Value,
                                        Mon = "",
                                        Rang = reglement.ACTI,
                                        Poste = reglement.POSTE,
                                        FinancementCategorie = reglement.CONVENTION + " " + reglement.CATEGORIE,
                                        Commune = reglement.GEO,
                                        Plan6 = reglement.PLAN6,
                                        Journal = journal,
                                        Marche = "",
                                        Status = etat

                                    });
                                }


                            }
                            catch (Exception ee) { }
                        }
                    }
                }
                #endregion
                #region Enregistrement donneur d'ordre
                bool test = saveDonneurOrdreBR(user, djournal, dateP);
                #endregion
                /*var afficheDOrdre = (from dord in db.OPA_DONNEURORDRE
                                     where dord.IDSOCIETE == user.IDSOCIETE
                                     select dord).First();
                this.tNomDOrdre.Text = afficheDOrdre.DONNEUR_ORDRE;

                this.tRIBdordre.Text = afficheDOrdre.CODE_BANQUE + afficheDOrdre.CODE_GUICHET + afficheDOrdre.NUM_COMPTE;*/
            }
            return list;
        }
        public OPA_DONNEURORDRE getDonneurOrdre(SI_USERS user)
        {
            SOFTCONNECTSIIG db = new SOFTCONNECTSIIG();
            return (from dor in db.OPA_DONNEURORDRE where dor.IDSOCIETE == user.IDPROJET select dor).FirstOrDefault();
        }
        private bool removePREGLEMENT(SI_USERS user)
        {
            SOFTCONNECTSIIG db = new SOFTCONNECTSIIG();
            bool test = false;

            List<OPA_REGLEMENT> nums = (from rglm in db.OPA_REGLEMENT
                                        where rglm.IDSOCIETE == user.IDPROJET
                                        select rglm).ToList();

            try
            {
                db.OPA_REGLEMENT.RemoveRange(nums);
                db.SaveChanges();
                test = true;
            }
            catch (Exception ex)
            {
                test = false;
            }
            return test;
        }
        private bool viderPAnomalie(SI_USERS user)
        {
            SOFTCONNECTSIIG db = new SOFTCONNECTSIIG();
            bool test = false;
            List<OPA_ANOMALIE> anoms = (from anomalie in db.OPA_ANOMALIE
                                        where anomalie.IDSOCIETE == user.IDPROJET
                                        select anomalie).ToList();

            foreach (var anom in anoms)
            {


                try
                {

                    db.OPA_ANOMALIE.Remove(anom);
                    db.SaveChanges();
                    test = true;
                }
                catch (Exception ex)
                {
                    test = false;
                }
            }
            return test;
        }
        private string formaterNORD(string NORD)
        {
            string nordFormate = "";
            string annee = NORD.Substring(0, 4);
            string mois = NORD.Substring(4, 2); ;
            string jour = NORD.Substring(6, 2); ;
            string numero = NORD.Substring(8, 6);
            nordFormate = jour + "/" + mois + "/" + annee + " - " + numero;

            return nordFormate;
        }
        private string formatTime(int i)
        {
            string r = i.ToString();
            if (r.Length < 2) r = "0" + r;
            return r;
        }
        private string recupDate(DateTime datetime)
        {
            return datetime.Day.ToString() + datetime.Month.ToString() + datetime.Year.ToString();
        }
        private string formatLibelle0702(string lib)
        {
            string libelle0702 = "";
            libelle0702 = lib.Substring(11, lib.Length - 11);
            libelle0702 = this.formaterTexte(11, libelle0702);
            return libelle0702;
        }
        private string formaterTexte(int x, string texte)
        {
            string s = ajouterText(x, texte);
            return couperText(x, s);
        }
        private string formaterChiffre(int x, string montant)
        {
            string mont = montant;
            int pos = mont.IndexOf(",");
            int suppr = mont.Length - (pos + 3);
            mont = mont.Replace(",", "");
            if (pos > 0)
            {
                mont = mont.Remove(pos + 2, suppr);
            }
            else
            {
                mont += "00";
            }

            mont = ajouter0(x, mont);
            return mont;
        }
        private string ajouter0(int x, string str)
        {

            string s = "";
            int n = 0;
            try
            {
                n = str.Length;
            }
            catch (Exception ex) { }
            if (n < x)
            {
                int y = x - n;
                s = str;
                for (int i = 0; i < y; i++)
                {
                    s = "0" + s;
                }
            }
            else
            {
                s = str;
            }
            return s;
        }
        private string ajouterText(int x, string str)
        {

            string s = "";
            int n = 0;
            try
            {
                if (str != null)
                {
                    n = str.Length;
                }

            }
            catch (Exception ex) { }
            if (n < x)
            {
                int y = x - n;
                s = str;
                for (int i = 0; i < y; i++)
                {
                    s += " ";
                }
            }
            else
            {
                s = str;
            }
            return s;
        }
        private string couperText(int x, string str)
        {
            string s = "";
            int n = 0;
            try
            {
                if (str != null)
                {
                    n = str.Length;
                }

            }
            catch (Exception ex) { }

            if (n > x)
            {
                int y = n - x;
                s = str.Remove(x, y);
            }
            else
            {
                s = str;
            }
            return s;
        }
        private string formaterDatePaie(DateTime date)
        {
            string day = date.Day.ToString();
            if (day.Length < 2)
            {
                day = "0" + day;
            }
            string mounth = date.Month.ToString();
            if (mounth.Length < 2)
            {
                mounth = "0" + mounth;
            }
            string year = date.Year.ToString();
            year = year.Substring(3, 1);
            string textdate = day + mounth + year;
            return this.couperText(5, textdate);
        }
        public void SaveValideSelectEcritureBR(List<string> numBR, string journal, string etat, bool devise, SI_USERS user)
        {
            SOFTCONNECTSIIG db = new SOFTCONNECTSIIG();
            SOFTCONNECTOM tom = new SOFTCONNECTOM();
            //bool test = false;
            List<int> supprLignes = new List<int>();
            RJL1 djournal = (from jrnl in tom.RJL1
                             where jrnl.CODE == journal
                             select jrnl).Single();
            foreach (string row in numBR)
            {

                string num = row;
                try
                {
                    var ecriture = (from mcpt in tom.MOP
                                    where mcpt.NUMEROOP == row && mcpt.COGE == djournal.COMPTEASSOCIE
                                    select new DataListTomOP()
                                    {
                                        No = mcpt.NUMEROOP,
                                        Date = mcpt.DATEFACTURE.Value,
                                        NoPiece = mcpt.NUMEROFACTURE,
                                        Compte = mcpt.COGEFOURNISSEUR,
                                        Libelle = mcpt.LIBELLE,
                                        Montant = mcpt.MONTANTLOC.Value,
                                        MontantDevise = mcpt.MONTANTDEV.Value,
                                        Mon = "",
                                        Rang = mcpt.ACTI,
                                        Poste = mcpt.POSTE,
                                        FinancementCategorie = mcpt.CONVENTION + " " + mcpt.CATEGORIE,
                                        Commune = mcpt.GEO,
                                        Plan6 = mcpt.PLAN6,
                                        Journal = journal,
                                        Marche = "",
                                        Auxi = mcpt.AUXIFOURNISSEUR
                                    }).FirstOrDefault();

                    var beneficiaire = (from bn in tom.RTIERS
                                        where bn.AUXI == ecriture.Auxi && bn.COGE == ecriture.Compte
                                        select new
                                        {
                                            BENEFICIAIRE = bn.NOM,
                                            BANQUE = bn.BQNOM,
                                            COMPTE_BANQUE = bn.RIB1,
                                            DOM1 = bn.DOM1,
                                            DOM2 = bn.DOM2
                                        }
                                        ).FirstOrDefault();
                    #region Sauve REGELEMENT & ANOMALIE

                    if (beneficiaire.COMPTE_BANQUE == null || beneficiaire.BENEFICIAIRE == null)
                    {
                        OPA_ANOMALIEBR panomalie = new OPA_ANOMALIEBR();
                        panomalie.NUM = ecriture.No;
                        panomalie.IDSOCIETE = user.IDPROJET;

                        try
                        {
                            db.OPA_ANOMALIEBR.Add(panomalie);
                            db.SaveChanges();
                        }
                        catch (Exception ee) { }
                    }
                    else
                    {
                        OPA_REGLEMENTBR preg = new OPA_REGLEMENTBR();
                        preg.NUM = ecriture.No;
                        preg.DATE = ecriture.Date;
                        preg.BENEFICIAIRE = beneficiaire.BENEFICIAIRE;
                        preg.BANQUE = beneficiaire.BANQUE;
                        preg.GUICHET = this.RIB(beneficiaire.COMPTE_BANQUE)[1];
                        preg.RIB = this.RIB(beneficiaire.COMPTE_BANQUE)[2];
                        preg.ETAT = etat;

                        if (devise)
                        {
                            preg.MONTANT = ecriture.Montant;
                        }
                        else
                        {
                            preg.MONTANT = ecriture.Montant;
                        }

                        preg.LIBELLE = this.formaterTexte(100, ecriture.No + ecriture.Libelle);
                        preg.NUM_ETABLISSEMENT = this.RIB(beneficiaire.COMPTE_BANQUE)[0];
                        preg.CODE_J = journal;
                        preg.DOM1 = beneficiaire.DOM1;
                        preg.DOM2 = beneficiaire.DOM2;
                        //preg.CATEGORIE = beneficiaire.CATEGORIE;
                        preg.APPLICATION = "BR";
                        preg.IDSOCIETE = user.IDPROJET;
                        try
                        {
                            db.OPA_REGLEMENTBR.Add(preg);
                            db.SaveChanges();
                        }
                        catch (Exception eee)
                        {
                        }
                    }
                    #endregion
                    //test = true;
                }
                catch (Exception ex)
                {
                    //test = false;
                }


            }

        }
        public void SaveValideSelectEcriturePaie(List<string> listReg, string journal, bool devise, SI_USERS user)
        {
            SOFTCONNECTSIIG db = new SOFTCONNECTSIIG();
            SOFTCONNECTOM tom = new SOFTCONNECTOM();
            //bool test = false;
            List<int> supprLignes = new List<int>();
            this.removePREGLEMENT(user);/*Mikajy 19/10/2023*/
            //this.viderPAnomalie(user);

            foreach (var row in listReg)
            {

                int num = Convert.ToInt32(row);
                try
                {
                    var beneficiaire = (from pre in tom.tpa_preparations
                                        join sal in tom.tpa_salaries on pre.matricule equals sal.matricule
                                        join ban in tom.tpa_BanqueSalaries on sal.matricule equals ban.matricule
                                        where pre.numeroautomatique == num
                                        select new DataListTompaie()
                                        {
                                            No = pre.numeroautomatique,
                                            Matricule = sal.matricule,
                                            Nom = sal.nom,
                                            Mois = pre.mois,
                                            Annee = pre.annee,
                                            Libelle = "Salaire du mois " + sal.matricule + pre.mois,
                                            Montant = pre.valeur.Value,
                                            Cin = sal.cin,
                                            Banque = ban.code_banque,
                                            CodeBanque = ban.codeLibelleBanque,
                                            Guichet = ban.codeGuichet,
                                            CompteBanque = ban.numCompte,
                                            CleRIB = ban.cle_RIB,

                                            /*No = pre.numeroautomatique,
                                            Matricule = sal.matricule,
                                            Nom = sal.nom,
                                            Mois = pre.mois,
                                            Annee = pre.annee,
                                            Libelle = "Salaire du mois " + sal.matricule + pre.mois,
                                            Montant = pre.valeur.Value,
                                            Cin = sal.cin,
                                            Banque = ban.code_etablissement,
                                            CodeBanque = ban.code_banque,
                                            Guichet = ban.codeGuichet,
                                            CompteBanque = ban.numCompte,
                                            CleRIB = ban.cle_RIB,*/
                                            //Date = pre.dateTraitement.Value,
                                            //Agence = ban.Agence
                                        }).FirstOrDefault();
                    /*var beneficiaire = (from mcpt in tom.MCOMPTA
                                        join rtier in tom.RTIERS on mcpt.COGEAUXI equals rtier.COGEAUXI
                                        where mcpt.NUMENREG == num 
                                        select new
                                        {
                                            NUM = mcpt.NUMENREG,
                                            NORD = mcpt.NORD,
                                            NOPIECE = mcpt.NOPIECE,
                                            COGE = mcpt.COGE,
                                            DATE = mcpt.DATCLE,
                                            MONTANT = mcpt.MONTANT,
                                            MONTDEV = mcpt.MONTDEV,
                                            DEVISE = mcpt.DEVISE,
                                            ACTI = mcpt.ACTI,
                                            POSTE = mcpt.POSTE,
                                            LIBELLE = mcpt.LIBELLE,
                                            CONVENTION = mcpt.CONVENTION,
                                            GEO = mcpt.GEO,
                                            PLAN6 = mcpt.PLAN6,
                                            JL = mcpt.JL,
                                            MARCHE = mcpt.MARCHE,
                                            CODE_J = mcpt.JL,
                                            CATEGORIE = mcpt.CATEGORIE,
                                            BENEFICIAIRE = rtier.NOM,
                                            BANQUE = rtier.BQNOM,
                                            COMPTE_BANQUE = rtier.RIB1,
                                            DOM1 = rtier.DOM1,
                                            DOM2 = rtier.DOM2
                                        }).FirstOrDefault();*/
                    /*var beneficiaire = (from bn in tom.MCOMPTA
                                        join rtier in tom.RTIERS on bn.COGEAUXI equals rtier.COGEAUXI
                                        where bn.NORD == ecriture.NORD
                                        select new
                                        {
                                            BENEFICIAIRE = rtier.NOM,
                                            BANQUE = rtier.BQNOM,
                                            COMPTE_BANQUE = rtier.RIB1,
                                            DOM1 = rtier.DOM1,
                                            DOM2 = rtier.DOM2
                                        }
                                        ).First();*/


                    #region Sauve REGELEMENT & ANOMALIE

                    if (beneficiaire.CompteBanque == null || beneficiaire.Nom == null)
                    {
                        OPA_ANOMALIE panomalie = new OPA_ANOMALIE();
                        panomalie.NUM = beneficiaire.No;

                        try
                        {
                            db.OPA_ANOMALIE.Add(panomalie);
                            db.SaveChanges();
                        }
                        catch (Exception ee) { }
                    }
                    else
                    {
                        OPA_REGLEMENT preg = new OPA_REGLEMENT();
                        preg.NUM = beneficiaire.No;
                        //preg.DATE = beneficiaire.Date;
                        preg.BENEFICIAIRE = beneficiaire.Nom;
                        preg.BANQUE = beneficiaire.Banque;
                        preg.GUICHET = beneficiaire.Guichet;
                        preg.RIB = beneficiaire.CompteBanque;
                        preg.APPLICATION = "PAIE";
                        preg.IDSOCIETE = user.IDPROJET;
                        if (devise)
                        {
                            preg.MONTANT = beneficiaire.Montant;
                        }
                        else
                        {
                            preg.MONTANT = beneficiaire.Montant;
                        }

                        preg.LIBELLE = beneficiaire.Libelle;
                        preg.NUM_ETABLISSEMENT = beneficiaire.CodeBanque;
                        preg.CODE_J = journal;
                        preg.DOM1 = beneficiaire.Nom;
                        //preg.DOM2 = beneficiaire.DOM2;
                        //preg.CATEGORIE = beneficiaire.CATEGORIE;
                        try
                        {
                            db.OPA_REGLEMENT.Add(preg);
                            db.SaveChanges();
                        }
                        catch (Exception eee)
                        {
                        }
                    }
                    #endregion
                    //test = true;
                }
                catch (Exception ex)
                {
                    //test = false;
                }


            }

        }
    }
}
