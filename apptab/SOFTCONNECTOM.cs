﻿using apptab.Models;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace apptab
{
    public partial class SOFTCONNECTOM : DbContext
    {
        public SOFTCONNECTOM()
            : base(connex)
        {
        }

        public static string connex = "name=SOFTCONNECTOM";
        public virtual DbSet<RPAYS> RPAYS { get; set; }
        public virtual DbSet<CPTADMIN_CHAINETRAITEMENT> CPTADMIN_CHAINETRAITEMENT { get; set; }
        public virtual DbSet<CPTADMIN_COMMENTAIRE> CPTADMIN_COMMENTAIRE { get; set; }
        public virtual DbSet<CPTADMIN_FLIQUIDATION> CPTADMIN_FLIQUIDATION { get; set; }
        public virtual DbSet<CPTADMIN_MLIQUIDATION> CPTADMIN_MLIQUIDATION { get; set; }
        public virtual DbSet<CPTADMIN_MLIQUIDATIONPJ> CPTADMIN_MLIQUIDATIONPJ { get; set; }
        public virtual DbSet<CPTADMIN_MODELEETATS> CPTADMIN_MODELEETATS { get; set; }
        public virtual DbSet<CPTADMIN_STRUCTURE> CPTADMIN_STRUCTURE { get; set; }
        public virtual DbSet<CPTADMIN_TRAITEMENT> CPTADMIN_TRAITEMENT { get; set; }
        public virtual DbSet<CPTADMIN_TYPEENGAGEMENT> CPTADMIN_TYPEENGAGEMENT { get; set; }
        public virtual DbSet<CPTADMIN_TYPEPROCEDURE> CPTADMIN_TYPEPROCEDURE { get; set; }

        public virtual DbSet<CPTADMIN_CHAINETRAITEMENT_AVANCE> CPTADMIN_CHAINETRAITEMENT_AVANCE { get; set; }
        public virtual DbSet<CPTADMIN_COMMENTAIRE_AVANCE> CPTADMIN_COMMENTAIRE_AVANCE { get; set; }
        public virtual DbSet<CPTADMIN_FAVANCE> CPTADMIN_FAVANCE { get; set; }
        public virtual DbSet<CPTADMIN_MAVANCE> CPTADMIN_MAVANCE { get; set; }
        public virtual DbSet<CPTADMIN_MAVANCEPJ> CPTADMIN_MAVANCEPJ { get; set; }
        public virtual DbSet<CPTADMIN_MODELEETATS_AVANCE> CPTADMIN_MODELEETATS_AVANCE { get; set; }
        public virtual DbSet<CPTADMIN_TRAITEMENT_AVANCE> CPTADMIN_TRAITEMENT_AVANCE { get; set; }

        public virtual DbSet<GA_AVANCE_JUSTIFICATIF> GA_AVANCE_JUSTIFICATIF { get; set; }
        public virtual DbSet<GA_AVANCE_REVERSEMENT> GA_AVANCE_REVERSEMENT { get; set; }

        public virtual DbSet<RTIERS> RTIERS { get; set; }
        public virtual DbSet<TP_MPIECES_JUSTIFICATIVES> TP_MPIECES_JUSTIFICATIVES { get; set; }
        public virtual DbSet<FCOMPTA> FCOMPTA { get; set; }
        public virtual DbSet<FOP> FOP { get; set; }
        public virtual DbSet<GA_AVANCE> GA_AVANCE { get; set; }
        public virtual DbSet<GA_AVANCE_MOUVEMENT> GA_AVANCE_MOUVEMENT { get; set; }
        public virtual DbSet<MCOMPTA> MCOMPTA { get; set; }
        public virtual DbSet<MOP> MOP { get; set; }
        public virtual DbSet<OP_CHAINETRAITEMENT> OP_CHAINETRAITEMENT { get; set; }
        public virtual DbSet<RCOMPTATRAIT> RCOMPTATRAIT { get; set; }
        public virtual DbSet<RJL1> RJL1 { get; set; }
        public virtual DbSet<RPROJET> RPROJET { get; set; }
        public virtual DbSet<tmp_bulletin> tmp_bulletin { get; set; }
        public virtual DbSet<tpa_BanqueSalaries> tpa_BanqueSalaries { get; set; }
        public virtual DbSet<tpa_preparations> tpa_preparations { get; set; }
        public virtual DbSet<tpa_salaries> tpa_salaries { get; set; }
        public virtual DbSet<CPTADMIN_FAUTREOPERATION> CPTADMIN_FAUTREOPERATION { get; set; }

        public virtual DbSet<MBUDALLOC> MBUDALLOC { get; set; }
        public virtual DbSet<MBUDGET> MBUDGET { get; set; }
        public virtual DbSet<RBUDGET> RBUDGET { get; set; }
        public virtual DbSet<RPOST1> RPOST1 { get; set; }
        public virtual DbSet<RREP> RREP { get; set; }
        public virtual DbSet<RREPACTI> RREPACTI { get; set; }
        public virtual DbSet<RREPACTIFINCATEG> RREPACTIFINCATEG { get; set; }
        public virtual DbSet<RREPFIN> RREPFIN { get; set; }
        public virtual DbSet<RREPGEO> RREPGEO { get; set; }
        public virtual DbSet<RREPPLAN6> RREPPLAN6 { get; set; }
        public virtual DbSet<RREPPOSTE> RREPPOSTE { get; set; }
        public virtual DbSet<RACTI1> RACTI1 { get; set; }
        public virtual DbSet<RCATEGORIE> RCATEGORIE { get; set; }
        public virtual DbSet<RCONVENTION> RCONVENTION { get; set; }
        public virtual DbSet<RGEO1> RGEO1 { get; set; }
        public virtual DbSet<RPLAN6> RPLAN6 { get; set; }
        public virtual DbSet<RSITE> RSITE { get; set; }

        public virtual DbSet<RBANQUES> RBANQUES { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CPTADMIN_FAUTREOPERATION>()
               .Property(e => e.COURSDEVISE)
               .HasPrecision(30, 12);

            modelBuilder.Entity<CPTADMIN_FAUTREOPERATION>()
                .Property(e => e.COURSRAPPORT)
                .HasPrecision(30, 12);

            modelBuilder.Entity<CPTADMIN_FAUTREOPERATION>()
                .Property(e => e.MONTANTLOCAL)
                .HasPrecision(30, 12);

            modelBuilder.Entity<CPTADMIN_FAUTREOPERATION>()
                .Property(e => e.MONTANTRAPPORT)
                .HasPrecision(30, 12);

            modelBuilder.Entity<CPTADMIN_FAUTREOPERATION>()
                .Property(e => e.MONTANTDEVISE)
                .HasPrecision(30, 12);

            modelBuilder.Entity<CPTADMIN_FLIQUIDATION>()
                .Property(e => e.COURSDEVISE)
                .HasPrecision(30, 12);

            modelBuilder.Entity<CPTADMIN_FLIQUIDATION>()
                .Property(e => e.COURSRAPPORT)
                .HasPrecision(30, 12);

            modelBuilder.Entity<CPTADMIN_FLIQUIDATION>()
                .HasMany(e => e.CPTADMIN_MLIQUIDATION)
                .WithRequired(e => e.CPTADMIN_FLIQUIDATION)
                .HasForeignKey(e => e.IDLIQUIDATION);

            modelBuilder.Entity<CPTADMIN_FLIQUIDATION>()
                .HasMany(e => e.CPTADMIN_MLIQUIDATIONPJ)
                .WithRequired(e => e.CPTADMIN_FLIQUIDATION)
                .HasForeignKey(e => e.IDLIQUIDATION);

            modelBuilder.Entity<CPTADMIN_MLIQUIDATION>()
                .Property(e => e.MONTANTLOCAL)
                .HasPrecision(30, 12);

            modelBuilder.Entity<CPTADMIN_MLIQUIDATION>()
                .Property(e => e.MONTANTRAPPORT)
                .HasPrecision(30, 12);

            modelBuilder.Entity<CPTADMIN_MLIQUIDATION>()
                .Property(e => e.MONTANTDEVISE)
                .HasPrecision(30, 12);

            modelBuilder.Entity<RTIERS>()
                .Property(e => e.NBJOURS)
                .HasPrecision(10, 0);

            modelBuilder.Entity<RTIERS>()
                .Property(e => e.JOURREF)
                .HasPrecision(10, 0);

            modelBuilder.Entity<RTIERS>()
                .Property(e => e.IMPORTID)
                .HasPrecision(18, 0);

            modelBuilder.Entity<TP_MPIECES_JUSTIFICATIVES>()
                .Property(e => e.NOMBRE)
                .HasPrecision(10, 0);

            modelBuilder.Entity<TP_MPIECES_JUSTIFICATIVES>()
                .Property(e => e.RANG)
                .HasPrecision(10, 0);

            modelBuilder.Entity<TP_MPIECES_JUSTIFICATIVES>()
                .Property(e => e.MONTANT)
                .HasPrecision(30, 6);

            modelBuilder.Entity<FCOMPTA>()
                .Property(e => e.NUMEROCHEQUE)
                .HasPrecision(18, 0);

            modelBuilder.Entity<FOP>()
                .Property(e => e.MONTANT)
                .HasPrecision(30, 6);

            modelBuilder.Entity<FOP>()
                .Property(e => e.MONTANTTOTALOPAVECOPENCOURSEXO)
                .HasPrecision(30, 6);

            modelBuilder.Entity<FOP>()
                .Property(e => e.MONTANTTOTALOPSANSOPENCOURSEXO)
                .HasPrecision(30, 6);

            modelBuilder.Entity<FOP>()
                .Property(e => e.MONTANTBUDGETDISPOEXO)
                .HasPrecision(30, 6);

            modelBuilder.Entity<FOP>()
                .Property(e => e.MONTANTBUDGETDISPOEXOAVANTOP)
                .HasPrecision(30, 6);

            modelBuilder.Entity<FOP>()
                .Property(e => e.MONTANTBUDGETDISPOPERIODE)
                .HasPrecision(30, 6);

            modelBuilder.Entity<FOP>()
                .Property(e => e.MONTANTBUDGETDISPOPERIODEAVANTOP)
                .HasPrecision(30, 6);

            modelBuilder.Entity<FOP>()
                .Property(e => e.MONTANTBUDGETDISPOSURMARCHE)
                .HasPrecision(30, 6);

            modelBuilder.Entity<FOP>()
                .Property(e => e.MONTANTMARCHEDISPOSUROP)
                .HasPrecision(30, 6);

            modelBuilder.Entity<FOP>()
                .Property(e => e.MONTANTMARCHEDISPOSUROPPERIODE)
                .HasPrecision(30, 6);

            modelBuilder.Entity<FOP>()
                .Property(e => e.MONTANTCOMMISSION)
                .HasPrecision(30, 6);

            modelBuilder.Entity<FOP>()
                .Property(e => e.TAXECOMMISSION)
                .HasPrecision(18, 6);

            modelBuilder.Entity<FOP>()
                .Property(e => e.COURSRAP)
                .HasPrecision(18, 6);

            modelBuilder.Entity<FOP>()
                .Property(e => e.COURSDEV)
                .HasPrecision(18, 6);

            modelBuilder.Entity<FOP>()
                .Property(e => e.MONTANTRAP)
                .HasPrecision(30, 6);

            modelBuilder.Entity<FOP>()
                .Property(e => e.MONTANTDEV)
                .HasPrecision(30, 6);

            modelBuilder.Entity<FOP>()
                .Property(e => e.MONTANTBUDGETEXO)
                .HasPrecision(30, 6);

            modelBuilder.Entity<FOP>()
                .Property(e => e.ENGAGEMENTSANTERIEURS)
                .HasPrecision(30, 6);

            modelBuilder.Entity<FOP>()
                .Property(e => e.MONTANTBUDGETPERIODE)
                .HasPrecision(30, 6);

            modelBuilder.Entity<FOP>()
                .Property(e => e.MONTANTTOTALOPSANSOPENCOURSPERIODE)
                .HasPrecision(30, 6);

            modelBuilder.Entity<FOP>()
                .Property(e => e.MONTANTTOTALOPAVECOPENCOURSPERIODE)
                .HasPrecision(30, 6);

            modelBuilder.Entity<FOP>()
                .Property(e => e.MARCHEMONTANTINITIAL)
                .HasPrecision(30, 6);

            modelBuilder.Entity<FOP>()
                .Property(e => e.MARCHEMONTANTOPANTERIEUR)
                .HasPrecision(30, 6);

            modelBuilder.Entity<FOP>()
                .Property(e => e.MARCHEMONTANTOPCUMULE)
                .HasPrecision(30, 6);

            modelBuilder.Entity<FOP>()
                .Property(e => e.MARCHESOLDEAPAYER)
                .HasPrecision(30, 6);

            modelBuilder.Entity<MCOMPTA>()
                .Property(e => e.NUMENREG)
                .HasPrecision(30, 0);

            modelBuilder.Entity<MCOMPTA>()
                .Property(e => e.MONTANT)
                .HasPrecision(30, 6);

            modelBuilder.Entity<MCOMPTA>()
                .Property(e => e.MONTDEV)
                .HasPrecision(30, 6);

            modelBuilder.Entity<MCOMPTA>()
                .Property(e => e.MTREPORT)
                .HasPrecision(30, 6);

            modelBuilder.Entity<MCOMPTA>()
                .Property(e => e.NLET)
                .HasPrecision(10, 0);

            modelBuilder.Entity<MCOMPTA>()
                .Property(e => e.NBORD)
                .HasPrecision(30, 6);

            modelBuilder.Entity<MCOMPTA>()
                .Property(e => e.RELEVE)
                .HasPrecision(10, 0);

            modelBuilder.Entity<MCOMPTA>()
                .Property(e => e.ANCIENDRF)
                .HasPrecision(10, 0);

            modelBuilder.Entity<MCOMPTA>()
                .Property(e => e.NOUVDRF)
                .HasPrecision(10, 0);

            modelBuilder.Entity<MCOMPTA>()
                .Property(e => e.MONTEMIS)
                .HasPrecision(30, 6);

            modelBuilder.Entity<MCOMPTA>()
                .Property(e => e.NPRELET)
                .HasPrecision(10, 0);

            modelBuilder.Entity<MCOMPTA>()
                .Property(e => e.QTE)
                .HasPrecision(30, 6);

            modelBuilder.Entity<MCOMPTA>()
                .Property(e => e.PU)
                .HasPrecision(30, 6);

            modelBuilder.Entity<MCOMPTA>()
                .Property(e => e.COURSREP)
                .HasPrecision(18, 6);

            modelBuilder.Entity<MCOMPTA>()
                .Property(e => e.COURSDEV)
                .HasPrecision(18, 6);

            modelBuilder.Entity<MCOMPTA>()
                .Property(e => e.NUMENGAG)
                .HasPrecision(18, 0);

            modelBuilder.Entity<MCOMPTA>()
                .Property(e => e.NOUVDRFCAA)
                .HasPrecision(10, 0);

            modelBuilder.Entity<MCOMPTA>()
                .Property(e => e.IMPORTIDH)
                .HasPrecision(18, 0);

            modelBuilder.Entity<MCOMPTA>()
                .Property(e => e.NUMENREGSITE)
                .HasPrecision(30, 0);

            modelBuilder.Entity<MOP>()
                .Property(e => e.MONTANTLOC)
                .HasPrecision(30, 6);

            modelBuilder.Entity<MOP>()
                .Property(e => e.MONTANTDEV)
                .HasPrecision(30, 6);

            modelBuilder.Entity<MOP>()
                .Property(e => e.MONTANTRAP)
                .HasPrecision(30, 6);

            modelBuilder.Entity<MOP>()
                .Property(e => e.MONTANTTVA)
                .HasPrecision(30, 6);

            modelBuilder.Entity<MOP>()
                .Property(e => e.MONTANTAUTRETAXE)
                .HasPrecision(30, 6);

            modelBuilder.Entity<MOP>()
                .Property(e => e.MONTANTRETENUE)
                .HasPrecision(30, 6);

            modelBuilder.Entity<MOP>()
                .Property(e => e.MONTANTTVADEV)
                .HasPrecision(30, 6);

            modelBuilder.Entity<MOP>()
                .Property(e => e.MONTANTTVARAP)
                .HasPrecision(30, 6);

            modelBuilder.Entity<MOP>()
                .Property(e => e.MONTANTAUTRETAXEDEV)
                .HasPrecision(30, 6);

            modelBuilder.Entity<MOP>()
                .Property(e => e.MONTANTAUTRETAXERAP)
                .HasPrecision(30, 6);

            modelBuilder.Entity<MOP>()
                .Property(e => e.MONTANTRETENUEDEV)
                .HasPrecision(30, 6);

            modelBuilder.Entity<MOP>()
                .Property(e => e.MONTANTRETENUERAP)
                .HasPrecision(30, 6);

            modelBuilder.Entity<MOP>()
                .Property(e => e.MONTANTCOMMISSION)
                .HasPrecision(30, 6);

            modelBuilder.Entity<MOP>()
                .Property(e => e.TAXECOMMISSION)
                .HasPrecision(30, 6);

            modelBuilder.Entity<RJL1>()
                .Property(e => e.NUMEROBR)
                .HasPrecision(18, 0);

            modelBuilder.Entity<RJL1>()
                .Property(e => e.INCREMENTATIONAUTO)
                .HasPrecision(10, 0);

            modelBuilder.Entity<RPROJET>()
                .Property(e => e.IDPROJET)
                .HasPrecision(30, 9);

            modelBuilder.Entity<RPROJET>()
                .Property(e => e.NOIMMO)
                .HasPrecision(10, 0);

            modelBuilder.Entity<RPROJET>()
                .Property(e => e.COURSREP)
                .HasPrecision(30, 9);

            modelBuilder.Entity<RPROJET>()
                .Property(e => e.NBJOURMIN1)
                .HasPrecision(10, 0);

            modelBuilder.Entity<RPROJET>()
                .Property(e => e.NBJOURMAX1)
                .HasPrecision(10, 0);

            modelBuilder.Entity<RPROJET>()
                .Property(e => e.NBJOURMIN2)
                .HasPrecision(10, 0);

            modelBuilder.Entity<RPROJET>()
                .Property(e => e.NBJOURMAX2)
                .HasPrecision(10, 0);

            modelBuilder.Entity<RPROJET>()
                .Property(e => e.NBJOURMIN3)
                .HasPrecision(10, 0);

            modelBuilder.Entity<RPROJET>()
                .Property(e => e.NBJOURMAX3)
                .HasPrecision(10, 0);

            modelBuilder.Entity<RPROJET>()
                .Property(e => e.NBJOURMIN4)
                .HasPrecision(10, 0);

            modelBuilder.Entity<RPROJET>()
                .Property(e => e.NBJOURMAX4)
                .HasPrecision(10, 0);

            modelBuilder.Entity<RPROJET>()
                .Property(e => e.NBJOURMIN5)
                .HasPrecision(10, 0);

            modelBuilder.Entity<RPROJET>()
                .Property(e => e.NBJOURMAX5)
                .HasPrecision(10, 0);

            modelBuilder.Entity<RPROJET>()
                .Property(e => e.NIVACTI)
                .HasPrecision(10, 0);

            modelBuilder.Entity<RPROJET>()
                .Property(e => e.INCREMENTAUTOMARCHE)
                .HasPrecision(2, 0);

            modelBuilder.Entity<RPROJET>()
                .Property(e => e.TAUX_TVA)
                .HasPrecision(30, 9);

            modelBuilder.Entity<RPROJET>()
                .Property(e => e.TAUX_AIR)
                .HasPrecision(30, 9);

            modelBuilder.Entity<RPROJET>()
                .Property(e => e.INCREMENTAUTOBC)
                .HasPrecision(2, 0);

            modelBuilder.Entity<RTIERS>()
                .Property(e => e.NBJOURS)
                .HasPrecision(10, 0);

            modelBuilder.Entity<RTIERS>()
                .Property(e => e.JOURREF)
                .HasPrecision(10, 0);

            modelBuilder.Entity<RTIERS>()
                .Property(e => e.IMPORTID)
                .HasPrecision(18, 0);

            modelBuilder.Entity<tmp_bulletin>()
                .Property(e => e._base)
                .HasPrecision(30, 9);

            modelBuilder.Entity<tmp_bulletin>()
                .Property(e => e.taux)
                .HasPrecision(30, 9);

            modelBuilder.Entity<tmp_bulletin>()
                .Property(e => e.gain)
                .HasPrecision(30, 9);

            modelBuilder.Entity<tmp_bulletin>()
                .Property(e => e.retenue)
                .HasPrecision(30, 9);

            modelBuilder.Entity<tmp_bulletin>()
                .Property(e => e.montant)
                .HasPrecision(30, 9);

            modelBuilder.Entity<tmp_bulletin>()
                .Property(e => e.typeligne)
                .IsFixedLength();

            modelBuilder.Entity<tmp_bulletin>()
                .Property(e => e.base1)
                .HasPrecision(30, 9);

            modelBuilder.Entity<tmp_bulletin>()
                .Property(e => e.taux1)
                .HasPrecision(30, 9);

            modelBuilder.Entity<tmp_bulletin>()
                .Property(e => e.gain1)
                .HasPrecision(30, 9);

            modelBuilder.Entity<tmp_bulletin>()
                .Property(e => e.retenue1)
                .HasPrecision(30, 9);

            modelBuilder.Entity<tpa_preparations>()
                .Property(e => e.numerodordre)
                .HasPrecision(18, 0);

            modelBuilder.Entity<tpa_preparations>()
                .Property(e => e._base)
                .HasPrecision(30, 9);

            modelBuilder.Entity<tpa_preparations>()
                .Property(e => e.taux)
                .HasPrecision(30, 9);

            modelBuilder.Entity<tpa_preparations>()
                .Property(e => e.valeur)
                .HasPrecision(30, 9);

            modelBuilder.Entity<tpa_preparations>()
                .Property(e => e.base1)
                .HasPrecision(30, 9);

            modelBuilder.Entity<tpa_preparations>()
                .Property(e => e.taux1)
                .HasPrecision(30, 9);

            modelBuilder.Entity<tpa_preparations>()
                .Property(e => e.valeur1)
                .HasPrecision(30, 9);

            modelBuilder.Entity<tpa_salaries>()
                .Property(e => e.sexe)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<tpa_salaries>()
                .Property(e => e.charge)
                .HasPrecision(18, 0);

            modelBuilder.Entity<tpa_salaries>()
                .Property(e => e.partIR)
                .HasPrecision(18, 5);

            modelBuilder.Entity<tpa_salaries>()
                .Property(e => e.salIndice)
                .HasPrecision(18, 5);

            modelBuilder.Entity<tpa_salaries>()
                .Property(e => e.salaireBase)
                .HasPrecision(18, 5);

            modelBuilder.Entity<tpa_salaries>()
                .Property(e => e.tauxHoraire)
                .HasPrecision(18, 5);

            modelBuilder.Entity<tpa_salaries>()
                .Property(e => e.taux)
                .HasPrecision(18, 3);

            modelBuilder.Entity<tpa_salaries>()
                .Property(e => e.taux2)
                .HasPrecision(18, 3);

            modelBuilder.Entity<tpa_salaries>()
                .Property(e => e.taux3)
                .HasPrecision(18, 3);

            modelBuilder.Entity<tpa_salaries>()
                .Property(e => e.taux4)
                .HasPrecision(18, 3);

            modelBuilder.Entity<tpa_salaries>()
                .Property(e => e.taux5)
                .HasPrecision(18, 3);

            modelBuilder.Entity<MCOMPTA>()
                .Property(e => e.NUMENREG)
                .HasPrecision(30, 0);

            modelBuilder.Entity<MCOMPTA>()
                .Property(e => e.MONTANT)
                .HasPrecision(30, 6);

            modelBuilder.Entity<MCOMPTA>()
                .Property(e => e.MONTDEV)
                .HasPrecision(30, 6);

            modelBuilder.Entity<MCOMPTA>()
                .Property(e => e.MTREPORT)
                .HasPrecision(30, 6);

            modelBuilder.Entity<MCOMPTA>()
                .Property(e => e.NLET)
                .HasPrecision(10, 0);

            modelBuilder.Entity<MCOMPTA>()
                .Property(e => e.NBORD)
                .HasPrecision(30, 6);

            modelBuilder.Entity<MCOMPTA>()
                .Property(e => e.RELEVE)
                .HasPrecision(10, 0);

            modelBuilder.Entity<MCOMPTA>()
                .Property(e => e.ANCIENDRF)
                .HasPrecision(10, 0);

            modelBuilder.Entity<MCOMPTA>()
                .Property(e => e.NOUVDRF)
                .HasPrecision(10, 0);

            modelBuilder.Entity<MCOMPTA>()
                .Property(e => e.MONTEMIS)
                .HasPrecision(30, 6);

            modelBuilder.Entity<MCOMPTA>()
                .Property(e => e.NPRELET)
                .HasPrecision(10, 0);

            modelBuilder.Entity<MCOMPTA>()
                .Property(e => e.QTE)
                .HasPrecision(30, 6);

            modelBuilder.Entity<MCOMPTA>()
                .Property(e => e.PU)
                .HasPrecision(30, 6);

            modelBuilder.Entity<MCOMPTA>()
                .Property(e => e.COURSREP)
                .HasPrecision(18, 6);

            modelBuilder.Entity<MCOMPTA>()
                .Property(e => e.COURSDEV)
                .HasPrecision(18, 6);

            modelBuilder.Entity<MCOMPTA>()
                .Property(e => e.NUMENGAG)
                .HasPrecision(18, 0);

            modelBuilder.Entity<MCOMPTA>()
                .Property(e => e.NOUVDRFCAA)
                .HasPrecision(10, 0);

            modelBuilder.Entity<MCOMPTA>()
                .Property(e => e.IMPORTIDH)
                .HasPrecision(18, 0);

            modelBuilder.Entity<MCOMPTA>()
                .Property(e => e.NUMENREGSITE)
                .HasPrecision(30, 0);

            modelBuilder.Entity<FCOMPTA>()
                .Property(e => e.NUMEROCHEQUE)
                .HasPrecision(18, 0);

            modelBuilder.Entity<FOP>()
                .Property(e => e.MONTANT)
                .HasPrecision(30, 6);

            modelBuilder.Entity<FOP>()
                .Property(e => e.MONTANTTOTALOPAVECOPENCOURSEXO)
                .HasPrecision(30, 6);

            modelBuilder.Entity<FOP>()
                .Property(e => e.MONTANTTOTALOPSANSOPENCOURSEXO)
                .HasPrecision(30, 6);

            modelBuilder.Entity<FOP>()
                .Property(e => e.MONTANTBUDGETDISPOEXO)
                .HasPrecision(30, 6);

            modelBuilder.Entity<FOP>()
                .Property(e => e.MONTANTBUDGETDISPOEXOAVANTOP)
                .HasPrecision(30, 6);

            modelBuilder.Entity<FOP>()
                .Property(e => e.MONTANTBUDGETDISPOPERIODE)
                .HasPrecision(30, 6);

            modelBuilder.Entity<FOP>()
                .Property(e => e.MONTANTBUDGETDISPOPERIODEAVANTOP)
                .HasPrecision(30, 6);

            modelBuilder.Entity<FOP>()
                .Property(e => e.MONTANTBUDGETDISPOSURMARCHE)
                .HasPrecision(30, 6);

            modelBuilder.Entity<FOP>()
                .Property(e => e.MONTANTMARCHEDISPOSUROP)
                .HasPrecision(30, 6);

            modelBuilder.Entity<FOP>()
                .Property(e => e.MONTANTMARCHEDISPOSUROPPERIODE)
                .HasPrecision(30, 6);

            modelBuilder.Entity<FOP>()
                .Property(e => e.MONTANTCOMMISSION)
                .HasPrecision(30, 6);

            modelBuilder.Entity<FOP>()
                .Property(e => e.TAXECOMMISSION)
                .HasPrecision(18, 6);

            modelBuilder.Entity<FOP>()
                .Property(e => e.COURSRAP)
                .HasPrecision(18, 6);

            modelBuilder.Entity<FOP>()
                .Property(e => e.COURSDEV)
                .HasPrecision(18, 6);

            modelBuilder.Entity<FOP>()
                .Property(e => e.MONTANTRAP)
                .HasPrecision(30, 6);

            modelBuilder.Entity<FOP>()
                .Property(e => e.MONTANTDEV)
                .HasPrecision(30, 6);

            modelBuilder.Entity<FOP>()
                .Property(e => e.ENGAGEMENTSANTERIEURS)
                .HasPrecision(30, 6);

            modelBuilder.Entity<FOP>()
                .Property(e => e.MONTANTBUDGETPERIODE)
                .HasPrecision(30, 6);

            modelBuilder.Entity<FOP>()
                .Property(e => e.MONTANTTOTALOPSANSOPENCOURSPERIODE)
                .HasPrecision(30, 6);

            modelBuilder.Entity<FOP>()
                .Property(e => e.MONTANTTOTALOPAVECOPENCOURSPERIODE)
                .HasPrecision(30, 6);

            modelBuilder.Entity<FOP>()
                .Property(e => e.MONTANTBUDGETEXO)
                .HasPrecision(30, 6);

            modelBuilder.Entity<FOP>()
                .Property(e => e.MARCHEMONTANTINITIAL)
                .HasPrecision(30, 6);

            modelBuilder.Entity<FOP>()
                .Property(e => e.MARCHEMONTANTOPANTERIEUR)
                .HasPrecision(30, 6);

            modelBuilder.Entity<FOP>()
                .Property(e => e.MARCHEMONTANTOPCUMULE)
                .HasPrecision(30, 6);

            modelBuilder.Entity<FOP>()
                .Property(e => e.MARCHESOLDEAPAYER)
                .HasPrecision(30, 6);

            modelBuilder.Entity<MCOMPTA>()
                .Property(e => e.NUMENREG)
                .HasPrecision(30, 0);

            modelBuilder.Entity<MCOMPTA>()
                .Property(e => e.MONTANT)
                .HasPrecision(30, 6);

            modelBuilder.Entity<MCOMPTA>()
                .Property(e => e.MONTDEV)
                .HasPrecision(30, 6);

            modelBuilder.Entity<MCOMPTA>()
                .Property(e => e.MTREPORT)
                .HasPrecision(30, 6);

            modelBuilder.Entity<MCOMPTA>()
                .Property(e => e.NLET)
                .HasPrecision(10, 0);

            modelBuilder.Entity<MCOMPTA>()
                .Property(e => e.NBORD)
                .HasPrecision(30, 6);

            modelBuilder.Entity<MCOMPTA>()
                .Property(e => e.RELEVE)
                .HasPrecision(10, 0);

            modelBuilder.Entity<MCOMPTA>()
                .Property(e => e.ANCIENDRF)
                .HasPrecision(10, 0);

            modelBuilder.Entity<MCOMPTA>()
                .Property(e => e.NOUVDRF)
                .HasPrecision(10, 0);

            modelBuilder.Entity<MCOMPTA>()
                .Property(e => e.MONTEMIS)
                .HasPrecision(30, 6);

            modelBuilder.Entity<MCOMPTA>()
                .Property(e => e.NPRELET)
                .HasPrecision(10, 0);

            modelBuilder.Entity<MCOMPTA>()
                .Property(e => e.QTE)
                .HasPrecision(30, 6);

            modelBuilder.Entity<MCOMPTA>()
                .Property(e => e.PU)
                .HasPrecision(30, 6);

            modelBuilder.Entity<MCOMPTA>()
                .Property(e => e.COURSREP)
                .HasPrecision(18, 6);

            modelBuilder.Entity<MCOMPTA>()
                .Property(e => e.COURSDEV)
                .HasPrecision(18, 6);

            modelBuilder.Entity<MCOMPTA>()
                .Property(e => e.NUMENGAG)
                .HasPrecision(18, 0);

            modelBuilder.Entity<MCOMPTA>()
                .Property(e => e.NOUVDRFCAA)
                .HasPrecision(10, 0);

            modelBuilder.Entity<MCOMPTA>()
                .Property(e => e.IMPORTIDH)
                .HasPrecision(18, 0);

            modelBuilder.Entity<MCOMPTA>()
                .Property(e => e.NUMENREGSITE)
                .HasPrecision(30, 0);

            modelBuilder.Entity<MOP>()
                .Property(e => e.MONTANTLOC)
                .HasPrecision(30, 6);

            modelBuilder.Entity<MOP>()
                .Property(e => e.MONTANTDEV)
                .HasPrecision(30, 6);

            modelBuilder.Entity<MOP>()
                .Property(e => e.MONTANTRAP)
                .HasPrecision(30, 6);

            modelBuilder.Entity<MOP>()
                .Property(e => e.MONTANTTVA)
                .HasPrecision(30, 6);

            modelBuilder.Entity<MOP>()
                .Property(e => e.MONTANTAUTRETAXE)
                .HasPrecision(30, 6);

            modelBuilder.Entity<MOP>()
                .Property(e => e.MONTANTRETENUE)
                .HasPrecision(30, 6);

            modelBuilder.Entity<MOP>()
                .Property(e => e.MONTANTTVADEV)
                .HasPrecision(30, 6);

            modelBuilder.Entity<MOP>()
                .Property(e => e.MONTANTTVARAP)
                .HasPrecision(30, 6);

            modelBuilder.Entity<MOP>()
                .Property(e => e.MONTANTAUTRETAXEDEV)
                .HasPrecision(30, 6);

            modelBuilder.Entity<MOP>()
                .Property(e => e.MONTANTAUTRETAXERAP)
                .HasPrecision(30, 6);

            modelBuilder.Entity<MOP>()
                .Property(e => e.MONTANTRETENUEDEV)
                .HasPrecision(30, 6);

            modelBuilder.Entity<MOP>()
                .Property(e => e.MONTANTRETENUERAP)
                .HasPrecision(30, 6);

            modelBuilder.Entity<MOP>()
                .Property(e => e.MONTANTCOMMISSION)
                .HasPrecision(30, 6);

            modelBuilder.Entity<MOP>()
                .Property(e => e.TAXECOMMISSION)
                .HasPrecision(30, 6);

            modelBuilder.Entity<RJL1>()
                .Property(e => e.NUMEROBR)
                .HasPrecision(18, 0);

            modelBuilder.Entity<RJL1>()
                .Property(e => e.INCREMENTATIONAUTO)
                .HasPrecision(10, 0);

            modelBuilder.Entity<RJL1>()
                .HasMany(e => e.MCOMPTA)
                .WithOptional(e => e.RJL1)
                .HasForeignKey(e => e.JL);

            modelBuilder.Entity<CPTADMIN_FAVANCE>()
                .Property(e => e.COURSDEVISE)
                .HasPrecision(30, 12);

            modelBuilder.Entity<CPTADMIN_FAVANCE>()
                .Property(e => e.COURSRAPPORT)
                .HasPrecision(30, 12);

            modelBuilder.Entity<CPTADMIN_FAVANCE>()
                .HasMany(e => e.CPTADMIN_MAVANCE)
                .WithRequired(e => e.CPTADMIN_FAVANCE)
                .HasForeignKey(e => e.IDAVANCE);

            modelBuilder.Entity<CPTADMIN_FAVANCE>()
                .HasMany(e => e.CPTADMIN_MAVANCEPJ)
                .WithRequired(e => e.CPTADMIN_FAVANCE)
                .HasForeignKey(e => e.IDAVANCE);

            modelBuilder.Entity<CPTADMIN_MAVANCE>()
                .Property(e => e.MONTANTLOCAL)
                .HasPrecision(30, 12);

            modelBuilder.Entity<CPTADMIN_MAVANCE>()
                .Property(e => e.MONTANTRAPPORT)
                .HasPrecision(30, 12);

            modelBuilder.Entity<CPTADMIN_MAVANCE>()
                .Property(e => e.MONTANTDEVISE)
                .HasPrecision(30, 12);

            modelBuilder.Entity<GA_AVANCE_JUSTIFICATIF>()
                .Property(e => e.MONTANT)
                .HasPrecision(18, 6);

            modelBuilder.Entity<GA_AVANCE_REVERSEMENT>()
                .Property(e => e.MONTANT)
                .HasPrecision(18, 6);

            modelBuilder.Entity<MBUDALLOC>()
                .Property(e => e.NUMBUD)
                .HasPrecision(10, 0);

            modelBuilder.Entity<MBUDALLOC>()
                .Property(e => e.NUMENREG)
                .HasPrecision(30, 0);

            modelBuilder.Entity<MBUDALLOC>()
                .Property(e => e.MONTANT)
                .HasPrecision(19, 4);

            modelBuilder.Entity<MBUDALLOC>()
                .Property(e => e.IMPORTID)
                .HasPrecision(18, 0);

            modelBuilder.Entity<MBUDGET>()
                .Property(e => e.NUMBUD)
                .HasPrecision(10, 0);

            modelBuilder.Entity<MBUDGET>()
                .Property(e => e.NUMENREG)
                .HasPrecision(30, 0);

            //modelBuilder.Entity<MBUDGET>()
            //    .Property(e => e.QUO)
            //    .HasPrecision(30, 6);

            //modelBuilder.Entity<MBUDGET>()
            //    .Property(e => e.PUO)
            //    .HasPrecision(19, 4);

            //modelBuilder.Entity<MBUDGET>()
            //    .Property(e => e.REPARTITION)
            //    .HasPrecision(5, 0);

            //modelBuilder.Entity<MBUDGET>()
            //    .Property(e => e.MONTBUDGET)
            //    .HasPrecision(19, 4);

            //modelBuilder.Entity<MBUDGET>()
            //    .Property(e => e.NBRREPF)
            //    .HasPrecision(5, 0);

            //modelBuilder.Entity<MBUDGET>()
            //    .Property(e => e.NBRREPD)
            //    .HasPrecision(5, 0);

            //modelBuilder.Entity<MBUDGET>()
            //    .Property(e => e.IMPORTID)
            //    .HasPrecision(18, 0);

            modelBuilder.Entity<RBUDGET>()
                .Property(e => e.NUMBUD)
                .HasPrecision(10, 0);

            modelBuilder.Entity<RBUDGET>()
                .Property(e => e.COURS)
                .HasPrecision(30, 6);

            modelBuilder.Entity<RBUDGET>()
                .Property(e => e.NIVPOST)
                .HasPrecision(10, 0);

            modelBuilder.Entity<RBUDGET>()
                .Property(e => e.NIVACTI)
                .HasPrecision(10, 0);

            modelBuilder.Entity<RBUDGET>()
                .Property(e => e.NIVGEO)
                .HasPrecision(10, 0);

            modelBuilder.Entity<RBUDGET>()
                .Property(e => e.NIVCOGE)
                .HasPrecision(10, 0);

            modelBuilder.Entity<RBUDGET>()
                .Property(e => e.NIVFIN)
                .HasPrecision(10, 0);

            modelBuilder.Entity<RBUDGET>()
                .Property(e => e.NIVPLAN6)
                .HasPrecision(10, 0);

            modelBuilder.Entity<RBUDGET>()
                .Property(e => e.LIMITE)
                .HasPrecision(18, 0);

            modelBuilder.Entity<RBUDGET>()
                .HasMany(e => e.MBUDGET)
                .WithRequired(e => e.RBUDGET)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<RPOST1>()
                .Property(e => e.NIVEAU)
                .HasPrecision(10, 0);

            modelBuilder.Entity<RPOST1>()
                .Property(e => e.STATUT)
                .HasPrecision(2, 0);

            modelBuilder.Entity<RPOST1>()
                .Property(e => e.PU)
                .HasPrecision(30, 6);

            modelBuilder.Entity<RPOST1>()
                .Property(e => e.IMPORTID)
                .HasPrecision(18, 0);

            modelBuilder.Entity<RPOST1>()
                .HasMany(e => e.MBUDGET)
                .WithOptional(e => e.RPOST1)
                .HasForeignKey(e => e.POSTE);

            modelBuilder.Entity<RREP>()
                .Property(e => e.MONTREP1)
                .HasPrecision(30, 6);

            modelBuilder.Entity<RREP>()
                .Property(e => e.MONTREP2)
                .HasPrecision(30, 6);

            modelBuilder.Entity<RREPACTI>()
                .Property(e => e.MONTREP1)
                .HasPrecision(25, 5);

            modelBuilder.Entity<RREPACTI>()
                .Property(e => e.MONTREP2)
                .HasPrecision(25, 5);

            modelBuilder.Entity<RREPACTIFINCATEG>()
                .Property(e => e.MONTREP1)
                .HasPrecision(30, 6);

            modelBuilder.Entity<RREPACTIFINCATEG>()
                .Property(e => e.MONTREP2)
                .HasPrecision(30, 6);

            modelBuilder.Entity<RREPACTIFINCATEG>()
                .Property(e => e.MONTLOC1)
                .HasPrecision(30, 6);

            modelBuilder.Entity<RREPACTIFINCATEG>()
                .Property(e => e.MONTLOC2)
                .HasPrecision(30, 6);

            modelBuilder.Entity<RREPFIN>()
                .Property(e => e.MONTREP1)
                .HasPrecision(25, 5);

            modelBuilder.Entity<RREPFIN>()
                .Property(e => e.MONTREP2)
                .HasPrecision(25, 5);

            modelBuilder.Entity<RREPGEO>()
                .Property(e => e.MONTREP1)
                .HasPrecision(25, 5);

            modelBuilder.Entity<RREPGEO>()
                .Property(e => e.MONTREP2)
                .HasPrecision(25, 5);

            modelBuilder.Entity<RREPPLAN6>()
                .Property(e => e.MONTREP1)
                .HasPrecision(25, 5);

            modelBuilder.Entity<RREPPLAN6>()
                .Property(e => e.MONTREP2)
                .HasPrecision(25, 5);

            modelBuilder.Entity<RREPPOSTE>()
                .Property(e => e.MONTREP1)
                .HasPrecision(25, 5);

            modelBuilder.Entity<RREPPOSTE>()
                .Property(e => e.MONTREP2)
                .HasPrecision(25, 5);

            modelBuilder.Entity<RACTI1>()
                .Property(e => e.NIVEAU)
                .HasPrecision(10, 0);

            modelBuilder.Entity<RACTI1>()
                .Property(e => e.PU)
                .HasPrecision(30, 6);

            modelBuilder.Entity<RACTI1>()
                .Property(e => e.IMPORTID)
                .HasPrecision(18, 0);

            modelBuilder.Entity<RCATEGORIE>()
                .Property(e => e.TAXE)
                .HasPrecision(2, 0);

            modelBuilder.Entity<RCATEGORIE>()
                .Property(e => e.MONTSEUILLOC)
                .HasPrecision(30, 6);

            modelBuilder.Entity<RCATEGORIE>()
                .Property(e => e.MONTSEUILDEV)
                .HasPrecision(30, 6);

            modelBuilder.Entity<RCONVENTION>()
                .Property(e => e.TAUXDRF)
                .HasPrecision(30, 6);

            modelBuilder.Entity<RCONVENTION>()
                .Property(e => e.TAUXSUIVI1)
                .HasPrecision(30, 6);

            modelBuilder.Entity<RCONVENTION>()
                .Property(e => e.TAUXSUIVI2)
                .HasPrecision(30, 6);

            modelBuilder.Entity<RCONVENTION>()
                .Property(e => e.TAUXDPD)
                .HasPrecision(30, 6);

            modelBuilder.Entity<RCONVENTION>()
                .Property(e => e.TAUXSUIVI1DPD)
                .HasPrecision(30, 6);

            modelBuilder.Entity<RCONVENTION>()
                .Property(e => e.TAUXSUIVI2DPD)
                .HasPrecision(30, 6);

            modelBuilder.Entity<RGEO1>()
                .Property(e => e.NIVEAU)
                .HasPrecision(10, 0);

            modelBuilder.Entity<RGEO1>()
                .Property(e => e.PU)
                .HasPrecision(30, 6);

            modelBuilder.Entity<RGEO1>()
                .Property(e => e.IMPORTID)
                .HasPrecision(18, 0);

            modelBuilder.Entity<RPLAN6>()
                .Property(e => e.NIVEAU)
                .HasPrecision(10, 0);

            modelBuilder.Entity<RPLAN6>()
                .Property(e => e.PU)
                .HasPrecision(30, 6);

            modelBuilder.Entity<RPLAN6>()
                .Property(e => e.IMPORTID)
                .HasPrecision(18, 0);
        }
    }
}
