BEGIN TRANSACTION;
DROP TABLE IF EXISTS "SolarSystems";
CREATE TABLE IF NOT EXISTS "SolarSystems" (
	"ssid"	INTEGER,
	"name"	TEXT UNIQUE,
	"startype"	TEXT NOT NULL,
	"sectorx"	INTEGER,
	"sectory"	INTEGER,
	"sectorz"	INTEGER,
	PRIMARY KEY("ssid")
);
DROP TABLE IF EXISTS "Playfields";
CREATE TABLE IF NOT EXISTS "Playfields" (
	"ssid"	INTEGER,
	"pfid"	INTEGER,
	"pftype"	INTEGER,
	"planettype"	TEXT,
	"name"	TEXT UNIQUE,
	"planetsize"	INTEGER,
	"maptype"	INTEGER,
	"iconcolor"	INTEGER,
	"isinstance"	BOOL,
	"ispvp"	BOOL,
	"sectorx"	INTEGER,
	"sectory"	INTEGER,
	"sectorz"	INTEGER,
	"posx"	REAL,
	"posy"	REAL,
	"posz"	REAL,
	PRIMARY KEY("pfid"),
	FOREIGN KEY("ssid") REFERENCES "SolarSystems"("ssid")
);
DROP TABLE IF EXISTS "Entities";
CREATE TABLE IF NOT EXISTS "Entities" (
	"entityid"	INTEGER,
	"pfid"	INTEGER NOT NULL,
	"name"	TEXT,
	"etype"	INTEGER,
	"facgroup"	INTEGER,
	"facid"	INTEGER,
	"posx"	REAL,
	"posy"	REAL,
	"posz"	REAL,
	"rotx"	REAL,
	"roty"	REAL,
	"rotz"	REAL,
	"health"	INTEGER,
	"isstructure"	BOOL,
	"isremoved"	BOOL DEFAULT 0,
	"removedticks"	INTEGER,
	"isproxy"	BOOL,
	"ispoi"	BOOL,
	"belongstoentityid"	INTEGER,
	"createdticks"	INTEGER DEFAULT 0,
	"isdead"	BOOL DEFAULT 0,
	"deadticks"	INTEGER,
	"dockedto"	INTEGER,
	"standingon"	INTEGER,
	"killedbyentityid"	INTEGER,
	PRIMARY KEY("entityid"),
	FOREIGN KEY("belongstoentityid") REFERENCES "Entities"("entityid"),
	FOREIGN KEY("killedbyentityid") REFERENCES "Entities"("entityid"),
	FOREIGN KEY("pfid") REFERENCES "Playfields"("pfid")
);
DROP TABLE IF EXISTS "Structures";
CREATE TABLE IF NOT EXISTS "Structures" (
	"entityid"	INTEGER NOT NULL,
	"lastvisitedticks"	INTEGER DEFAULT 0,
	"cntblocks"	INTEGER DEFAULT -1,
	"cntdevices"	INTEGER DEFAULT -1,
	"cnttriangles"	INTEGER DEFAULT -1,
	"cntlights"	INTEGER DEFAULT -1,
	"classnr"	INTEGER DEFAULT -1,
	"ispowered"	BOOL DEFAULT 0,
	"fuel"	INTEGER DEFAULT 0,
	"coretype"	INTEGER DEFAULT -1,
	"pilotId"	INTEGER,
	"hasteleporter"	BOOL DEFAULT 0,
	"hasspunlocked"	BOOL DEFAULT 0,
	"hassplocked"	BOOL DEFAULT 0,
	"playercreated"	BOOL DEFAULT 0,
	"frombplibrary"	BOOL DEFAULT 0,
	"discovery"	BOOL DEFAULT 0,
	"stationinterface"	BOOL DEFAULT 0,
	"sizex"	INTEGER DEFAULT 0,
	"sizey"	INTEGER DEFAULT 0,
	"bpname"	TEXT,
	PRIMARY KEY("entityid"),
	FOREIGN KEY("entityid") REFERENCES "Entities"("entityid"),
	FOREIGN KEY("pilotId") REFERENCES "Entities"("entityid")
);
DROP TABLE IF EXISTS "StructuresHistory";
CREATE TABLE IF NOT EXISTS "StructuresHistory" (
	"shid"	INTEGER NOT NULL,
	"entityid"	INTEGER NOT NULL,
	"name"	TEXT,
	"pfid"	INTEGER,
	"gametime"	INTEGER,
	"facgroup"	INTEGER,
	"facid"	INTEGER,
	"posx"	REAL,
	"posy"	REAL,
	"posz"	REAL,
	"rotx"	REAL,
	"roty"	REAL,
	"rotz"	REAL,
	"cntblocks"	INTEGER,
	"cntdevices"	INTEGER,
	"cntcores"	INTEGER,
	"ispowered"	BOOL DEFAULT 0,
	"dockedto"	INTEGER,
	"touchedgametime"	INTEGER,
	"touchedentityid"	INTEGER,
	"removedinfo"	TEXT,
	PRIMARY KEY("shid"),
	FOREIGN KEY("pfid") REFERENCES "Playfields"("pfid"),
	FOREIGN KEY("entityid") REFERENCES "Entities"("entityid"),
	FOREIGN KEY("touchedentityid") REFERENCES "Entities"("entityid")
);
DROP TABLE IF EXISTS "StructuresDeviceCount";
CREATE TABLE IF NOT EXISTS "StructuresDeviceCount" (
	"entityid"	INTEGER NOT NULL,
	"deviceid"	TEXT,
	"count"	INTEGER,
	PRIMARY KEY("entityid","deviceid"),
	FOREIGN KEY("entityid") REFERENCES "Entities"("entityid")
);
DROP TABLE IF EXISTS "PlayfieldResources";
CREATE TABLE IF NOT EXISTS "PlayfieldResources" (
	"localid"	INTEGER NOT NULL,
	"pfid"	INTEGER NOT NULL,
	"blockid"	INTEGER NOT NULL,
	"blockx"	INTEGER NOT NULL,
	"blocky"	INTEGER NOT NULL,
	"blockz"	INTEGER NOT NULL,
	"radius"	INTEGER NOT NULL,
	"totalblocks"	INTEGER NOT NULL,
	"remainingblocks"	INTEGER NOT NULL,
	PRIMARY KEY("pfid","blockx","blocky","blockz"),
	FOREIGN KEY("pfid") REFERENCES "Playfields"("pfid")
);
DROP TABLE IF EXISTS "TerrainPlaceables";
CREATE TABLE IF NOT EXISTS "TerrainPlaceables" (
	"type"	INTEGER NOT NULL,
	"pfid"	INTEGER NOT NULL,
	"entityid"	INTEGER NOT NULL,
	"blockid"	INTEGER NOT NULL,
	"blockx"	INTEGER NOT NULL,
	"blocky"	INTEGER NOT NULL,
	"blockz"	INTEGER NOT NULL,
	"facaccess"	BOOL NOT NULL,
	"tpentityid"	INTEGER,
	PRIMARY KEY("pfid","blockx","blocky","blockz"),
	FOREIGN KEY("entityid") REFERENCES "Entities"("entityid"),
	FOREIGN KEY("pfid") REFERENCES "Playfields"("pfid"),
	FOREIGN KEY("tpentityid") REFERENCES "Entities"("entityid")
);
DROP TABLE IF EXISTS "TraderHistory";
CREATE TABLE IF NOT EXISTS "TraderHistory" (
	"thid"	INTEGER NOT NULL,
	"gametime"	INTEGER NOT NULL,
	"entityid"	INTEGER NOT NULL,
	"pfid"	INTEGER NOT NULL,
	"poiid"	INTEGER NOT NULL,
	"tradername"	TEXT NOT NULL,
	"blockx"	INTEGER,
	"blocky"	INTEGER,
	"blockz"	INTEGER,
	"type"	INTEGER,
	"itemid"	INTEGER,
	"count"	INTEGER,
	"totalprice"	REAL,
	PRIMARY KEY("thid"),
	FOREIGN KEY("poiid") REFERENCES "Entities"("entityid"),
	FOREIGN KEY("entityid") REFERENCES "Entities"("entityid")
);
DROP TABLE IF EXISTS "DiscoveredPlayfields";
CREATE TABLE IF NOT EXISTS "DiscoveredPlayfields" (
	"facgroup"	INTEGER NOT NULL,
	"facid"	INTEGER NOT NULL,
	"entityid"	INTEGER NOT NULL,
	"pfid"	INTEGER NOT NULL,
	"gametime"	INTEGER,
	PRIMARY KEY("facgroup","facid","entityid","pfid"),
	FOREIGN KEY("entityid") REFERENCES "Entities"("entityid"),
	FOREIGN KEY("pfid") REFERENCES "Playfields"("pfid")
);
DROP TABLE IF EXISTS "ChangedPlayfields";
CREATE TABLE IF NOT EXISTS "ChangedPlayfields" (
	"cid"	INTEGER NOT NULL,
	"type"	INTEGER NOT NULL,
	"entityid"	INTEGER NOT NULL,
	"attentityid"	INTEGER,
	"frompfid"	INTEGER NOT NULL,
	"topfid"	INTEGER NOT NULL,
	"fromposx"	REAL,
	"fromposy"	REAL,
	"fromposz"	REAL,
	"toposx"	REAL,
	"toposy"	REAL,
	"toposz"	REAL,
	"aspassenger"	BOOL,
	"spawnat"	TEXT,
	"gametime"	INTEGER,
	PRIMARY KEY("cid"),
	FOREIGN KEY("topfid") REFERENCES "Playfields"("pfid"),
	FOREIGN KEY("attentityid") REFERENCES "Entities"("entityid"),
	FOREIGN KEY("frompfid") REFERENCES "Playfields"("pfid"),
	FOREIGN KEY("entityid") REFERENCES "Entities"("entityid")
);
DROP TABLE IF EXISTS "Bookmarks";
CREATE TABLE IF NOT EXISTS "Bookmarks" (
	"bid"	INTEGER NOT NULL,
	"type"	INTEGER NOT NULL,
	"refid"	INTEGER,
	"facgroup"	INTEGER NOT NULL,
	"facid"	INTEGER NOT NULL,
	"entityid"	INTEGER NOT NULL,
	"pfid"	INTEGER,
	"name"	TEXT,
	"sectorx"	INTEGER,
	"sectory"	INTEGER,
	"sectorz"	INTEGER,
	"posx"	REAL,
	"posy"	REAL,
	"posz"	REAL,
	"icon"	INTEGER,
	"isshared"	BOOL,
	"iswaypoint"	BOOL,
	"isremove"	BOOL,
	"isshowhud"	BOOL,
	"iscallback"	BOOL,
	"createdticks"	INTEGER NOT NULL,
	"expireafterticks"	INTEGER DEFAULT 0,
	"mindistance"	INTEGER DEFAULT 0,
	"maxdistance"	INTEGER DEFAULT -1,
	PRIMARY KEY("bid"),
	FOREIGN KEY("pfid") REFERENCES "Playfields"("pfid"),
	FOREIGN KEY("entityid") REFERENCES "Entities"("entityid")
);
DROP TABLE IF EXISTS "DiscoveredPOIs";
CREATE TABLE IF NOT EXISTS "DiscoveredPOIs" (
	"facgroup"	INTEGER NOT NULL,
	"facid"	INTEGER NOT NULL,
	"entityid"	INTEGER NOT NULL,
	"poitype"	INTEGER NOT NULL,
	"poiid"	INTEGER NOT NULL,
	"pfid"	INTEGER NOT NULL,
	"gametime"	INTEGER DEFAULT 0,
	PRIMARY KEY("facgroup","facid","poitype","poiid","pfid"),
	FOREIGN KEY("entityid") REFERENCES "Entities"("entityid"),
	FOREIGN KEY("pfid") REFERENCES "Playfields"("pfid")
);
DROP TABLE IF EXISTS "VisitedStructures";
CREATE TABLE IF NOT EXISTS "VisitedStructures" (
	"vp"	INTEGER NOT NULL,
	"entityid"	INTEGER NOT NULL,
	"poiid"	INTEGER NOT NULL,
	"gametime"	INTEGER,
	PRIMARY KEY("vp"),
	FOREIGN KEY("poiid") REFERENCES "Entities"("entityid"),
	FOREIGN KEY("entityid") REFERENCES "Entities"("entityid")
);
DROP TABLE IF EXISTS "PlayerStatistics";
CREATE TABLE IF NOT EXISTS "PlayerStatistics" (
	"entityid"	INTEGER NOT NULL,
	"killedenemies"	INTEGER,
	"killedallied"	INTEGER,
	"killedanimals"	INTEGER,
	"killeddrones"	INTEGER,
	"killedplayers"	INTEGER,
	"killedalliedplayers"	INTEGER,
	"died"	INTEGER,
	"score"	INTEGER,
	"blocksplaced"	INTEGER,
	"blocksdigged"	INTEGER,
	"walkedmeters"	INTEGER,
	"godmodemeters"	INTEGER,
	"jetpackmeters"	INTEGER,
	"hvmeters"	INTEGER,
	"svmeters"	INTEGER,
	"cvmeters"	INTEGER,
	"playerbikemeters"	INTEGER,
	"playtime"	REAL,
	"travly"	REAL DEFAULT 0,
	"travau"	REAL DEFAULT 0,
	PRIMARY KEY("entityid"),
	FOREIGN KEY("entityid") REFERENCES "Entities"("entityid")
);
DROP TABLE IF EXISTS "PlayerStatisticsOres";
CREATE TABLE IF NOT EXISTS "PlayerStatisticsOres" (
	"entityid"	INTEGER NOT NULL,
	"id"	INTEGER,
	"count"	INTEGER,
	PRIMARY KEY("entityid","id"),
	FOREIGN KEY("entityid") REFERENCES "Entities"("entityid")
);
DROP TABLE IF EXISTS "PlayerStatisticsCores";
CREATE TABLE IF NOT EXISTS "PlayerStatisticsCores" (
	"entityid"	INTEGER NOT NULL,
	"id"	INTEGER,
	"count"	INTEGER,
	PRIMARY KEY("entityid","id"),
	FOREIGN KEY("entityid") REFERENCES "Entities"("entityid")
);
DROP TABLE IF EXISTS "PlayerStatisticsPDAChapters";
CREATE TABLE IF NOT EXISTS "PlayerStatisticsPDAChapters" (
	"pspid"	INTEGER,
	"entityid"	INTEGER NOT NULL,
	"chapterid"	TEXT,
	"title"	TEXT,
	"gametime"	INTEGER,
	PRIMARY KEY("pspid"),
	FOREIGN KEY("entityid") REFERENCES "Entities"("entityid")
);
DROP TABLE IF EXISTS "PlayerStatisticsAIVessels";
CREATE TABLE IF NOT EXISTS "PlayerStatisticsAIVessels" (
	"psaid"	INTEGER,
	"type"	INTEGER,
	"entityid"	INTEGER NOT NULL,
	"vesselid"	INTEGER NOT NULL,
	"gametime"	INTEGER,
	PRIMARY KEY("psaid"),
	FOREIGN KEY("entityid") REFERENCES "Entities"("entityid"),
	FOREIGN KEY("vesselid") REFERENCES "Entities"("entityid")
);
DROP TABLE IF EXISTS "PlayerInventory";
CREATE TABLE IF NOT EXISTS "PlayerInventory" (
	"piid"	INTEGER,
	"entityid"	INTEGER NOT NULL,
	"gametime"	INTEGER NOT NULL,
	PRIMARY KEY("piid"),
	FOREIGN KEY("entityid") REFERENCES "Entities"("entityid")
);
DROP TABLE IF EXISTS "PlayerInventoryItems";
CREATE TABLE IF NOT EXISTS "PlayerInventoryItems" (
	"id"	INTEGER NOT NULL,
	"piid"	INTEGER NOT NULL,
	"item"	INTEGER NOT NULL,
	"count"	INTEGER NOT NULL,
	PRIMARY KEY("id"),
	FOREIGN KEY("piid") REFERENCES "PlayerInventory"("piid")
);
DROP TABLE IF EXISTS "DiscoveredFactions";
CREATE TABLE IF NOT EXISTS "DiscoveredFactions" (
	"facgroup"	INTEGER NOT NULL,
	"facid"	INTEGER NOT NULL,
	"disfacgroup"	INTEGER NOT NULL,
	"disfacid"	INTEGER NOT NULL,
	"gametime"	INTEGER NOT NULL,
	PRIMARY KEY("facgroup","facid","disfacgroup","disfacid")
);
DROP TABLE IF EXISTS "LoginLogoff";
CREATE TABLE IF NOT EXISTS "LoginLogoff" (
	"lid"	INTEGER NOT NULL,
	"entityid"	INTEGER NOT NULL,
	"playerid"	TEXT NOT NULL,
	"playername"	TEXT NOT NULL,
	"clientid"	INTEGER NOT NULL,
	"loginticks"	INTEGER,
	"logoffticks"	INTEGER,
	"buildnr"	INTEGER,
	"ip"	TEXT,
	"dram"	INTEGER,
	"vram"	INTEGER,
	"os"	TEXT,
	"gfx"	TEXT,
	"cpu"	TEXT,
	"cpufreq"	INTEGER,
	"cpucores"	TEXT,
	"culture"	TEXT,
	"timezone"	TEXT,
	"controller"	BOOL,
	"fsize"	INTEGER,
	PRIMARY KEY("lid"),
	FOREIGN KEY("entityid") REFERENCES "Entities"("entityid")
);
DROP TABLE IF EXISTS "ServerStartStop";
CREATE TABLE IF NOT EXISTS "ServerStartStop" (
	"sid"	INTEGER NOT NULL,
	"startticks"	INTEGER,
	"stopticks"	INTEGER,
	"starttime"	TEXT,
	"stoptime"	TEXT,
	"version"	TEXT,
	"build"	INTEGER,
	"timezone"	TEXT,
	PRIMARY KEY("sid")
);
DROP TABLE IF EXISTS "PlayerDeaths";
CREATE TABLE IF NOT EXISTS "PlayerDeaths" (
	"pid"	INTEGER NOT NULL,
	"entityid"	INTEGER NOT NULL,
	"attentityid"	INTEGER,
	"secondslived"	INTEGER NOT NULL,
	"killerid"	INTEGER,
	"killershipid"	INTEGER,
	"reason"	INTEGER NOT NULL,
	"pfid"	INTEGER NOT NULL,
	"gametime"	INTEGER NOT NULL,
	"posx"	REAL,
	"posy"	REAL,
	"posz"	REAL,
	PRIMARY KEY("pid"),
	FOREIGN KEY("attentityid") REFERENCES "Entities"("entityid"),
	FOREIGN KEY("killerid") REFERENCES "Entities"("entityid"),
	FOREIGN KEY("entityid") REFERENCES "Entities"("entityid"),
	FOREIGN KEY("killershipid") REFERENCES "Entities"("entityid"),
	FOREIGN KEY("pfid") REFERENCES "Playfields"("pfid")
);
DROP TABLE IF EXISTS "PlayerLevelUp";
CREATE TABLE IF NOT EXISTS "PlayerLevelUp" (
	"plid"	INTEGER NOT NULL,
	"entityid"	INTEGER NOT NULL,
	"level"	INTEGER,
	"exppoints"	INTEGER NOT NULL,
	"gametime"	INTEGER NOT NULL,
	PRIMARY KEY("plid"),
	FOREIGN KEY("entityid") REFERENCES "Entities"("entityid")
);
DROP TABLE IF EXISTS "PlayerPosHistory";
CREATE TABLE IF NOT EXISTS "PlayerPosHistory" (
	"ppid"	INTEGER NOT NULL,
	"entityid"	INTEGER NOT NULL,
	"attentityid"	INTEGER,
	"pfid"	INTEGER NOT NULL,
	"posx"	INTEGER,
	"posy"	INTEGER,
	"posz"	INTEGER,
	"gametime"	INTEGER NOT NULL,
	PRIMARY KEY("ppid"),
	FOREIGN KEY("entityid") REFERENCES "Entities"("entityid"),
	FOREIGN KEY("attentityid") REFERENCES "Entities"("entityid"),
	FOREIGN KEY("pfid") REFERENCES "Playfields"("pfid")
);
DROP TABLE IF EXISTS "DialogueVars";
CREATE TABLE IF NOT EXISTS "DialogueVars" (
	"type"	INTEGER NOT NULL,
	"name"	TEXT NOT NULL,
	"value"	INTEGER,
	"valuestr"	TEXT,
	"entityid"	INTEGER NOT NULL,
	"state"	TEXT NOT NULL,
	PRIMARY KEY("state","name","entityid")
);
DROP TABLE IF EXISTS "DialogueVisitedStates";
CREATE TABLE IF NOT EXISTS "DialogueVisitedStates" (
	"state"	TEXT NOT NULL,
	"count"	INTEGER NOT NULL,
	PRIMARY KEY("state")
);
DROP TABLE IF EXISTS "PlayerData";
CREATE TABLE IF NOT EXISTS "PlayerData" (
	"entityid"	INTEGER NOT NULL,
	"pfid"	INTEGER,
	"posx"	REAL,
	"posy"	REAL,
	"posz"	REAL,
	"rotx"	REAL,
	"roty"	REAL,
	"rotz"	REAL,
	"credits"	INTEGER,
	PRIMARY KEY("entityid"),
	FOREIGN KEY("pfid") REFERENCES "Playfields"("pfid"),
	FOREIGN KEY("entityid") REFERENCES "Entities"("entityid")
);
DROP TABLE IF EXISTS "PlayerSkillValues";
CREATE TABLE IF NOT EXISTS "PlayerSkillValues" (
	"entityid"	INTEGER NOT NULL,
	"name"	TEXT NOT NULL,
	"value"	REAL,
	PRIMARY KEY("entityid","name"),
	FOREIGN KEY("entityid") REFERENCES "Entities"("entityid")
);
DROP TABLE IF EXISTS "PerformanceData";
CREATE TABLE IF NOT EXISTS "PerformanceData" (
	"pdid"	INTEGER NOT NULL,
	"type"	INTEGER,
	"uptime"	INTEGER,
	"gametime"	INTEGER,
	"fps"	INTEGER,
	"fpsmin"	INTEGER,
	"heapmem"	INTEGER,
	"maxheapmem"	INTEGER,
	"systemmem"	INTEGER,
	"freemem"	INTEGER,
	"pfserver"	INTEGER,
	"playfields"	INTEGER,
	"processid"	INTEGER,
	"pfid"	INTEGER,
	"entityid"	INTEGER,
	"posx"	INTEGER,
	"posy"	INTEGER,
	"posz"	INTEGER,
	"ping"	INTEGER DEFAULT -1,
	"chunks"	INTEGER,
	"structs"	INTEGER,
	"proxies"	INTEGER,
	"players"	INTEGER,
	"mobs"	INTEGER,
	"dbqueries"	INTEGER,
	"dbresults"	INTEGER,
	"hitasks"	INTEGER,
	"lotasks"	INTEGER,
	"dbtasks"	INTEGER,
	"queue0"	INTEGER,
	"queue1"	INTEGER,
	"queue2"	INTEGER,
	"queue3"	INTEGER,
	PRIMARY KEY("pdid"),
	FOREIGN KEY("entityid") REFERENCES "Entities"("entityid"),
	FOREIGN KEY("pfid") REFERENCES "Playfields"("pfid")
);
DROP TABLE IF EXISTS "PerformanceDBResults";
CREATE TABLE IF NOT EXISTS "PerformanceDBResults" (
	"pdid"	INTEGER NOT NULL,
	"type"	INTEGER,
	"gametime"	INTEGER,
	"query"	TEXT,
	"count"	INTEGER,
	"results"	INTEGER,
	PRIMARY KEY("pdid")
);
DROP TABLE IF EXISTS "PerformanceNWPackages";
CREATE TABLE IF NOT EXISTS "PerformanceNWPackages" (
	"pnid"	INTEGER NOT NULL,
	"type"	INTEGER,
	"packageid"	INTEGER,
	"name"	TEXT,
	"readcount"	INTEGER,
	"readlength"	INTEGER,
	"writecount"	INTEGER,
	"writelength"	INTEGER,
	"gametime"	INTEGER,
	PRIMARY KEY("pnid")
);
DROP TABLE IF EXISTS "ChatMessages";
CREATE TABLE IF NOT EXISTS "ChatMessages" (
	"cmid"	INTEGER NOT NULL,
	"gametime"	INTEGER,
	"sendertype"	INTEGER,
	"channel"	INTEGER,
	"senderentityid"	INTEGER,
	"senderfacgroup"	INTEGER,
	"senderfacid"	INTEGER,
	"sendername"	TEXT,
	"recentityid"	INTEGER,
	"recfacgroup"	INTEGER,
	"recfacid"	INTEGER,
	"istextlocakey"	BOOL,
	"text"	TEXT NOT NULL,
	"arg1"	TEXT,
	"arg2"	TEXT,
	PRIMARY KEY("cmid"),
	FOREIGN KEY("recentityid") REFERENCES "Entities"("entityid"),
	FOREIGN KEY("senderentityid") REFERENCES "Entities"("entityid")
);
DROP TABLE IF EXISTS "Marketplace";
CREATE TABLE IF NOT EXISTS "Marketplace" (
	"mpid"	INTEGER NOT NULL,
	"isforsale"	BOOL,
	"iscreatorclaimed"	BOOL,
	"isacceptorclaimed"	BOOL,
	"isaborted"	BOOL,
	"type"	INTEGER,
	"count"	INTEGER,
	"price"	REAL,
	"creatorentityid"	INTEGER,
	"acceptorentityid"	INTEGER,
	"stationentityid"	INTEGER NOT NULL,
	"endtime"	INTEGER,
	"transactiontime"	INTEGER,
	"acceptedendtime"	INTEGER,
	"maxtranscost"	INTEGER,
	PRIMARY KEY("mpid"),
	FOREIGN KEY("acceptorentityid") REFERENCES "Entities"("entityid"),
	FOREIGN KEY("creatorentityid") REFERENCES "Entities"("entityid"),
	FOREIGN KEY("stationentityid") REFERENCES "Entities"("entityid")
);
DROP TABLE IF EXISTS "StationServicesHistory";
CREATE TABLE IF NOT EXISTS "StationServicesHistory" (
	"shid"	INTEGER NOT NULL,
	"gametime"	INTEGER,
	"playerid"	INTEGER NOT NULL,
	"shipid"	INTEGER NOT NULL,
	"stationid"	INTEGER NOT NULL,
	"pfid"	INTEGER NOT NULL,
	"fuel"	INTEGER,
	"fuelcost"	INTEGER,
	"o2"	INTEGER,
	"o2cost"	INTEGER,
	"warp"	INTEGER,
	"warpcost"	INTEGER,
	"ammo"	INTEGER,
	"ammocost"	INTEGER,
	"shield"	INTEGER,
	"shieldcost"	INTEGER,
	"repaircost"	INTEGER,
	PRIMARY KEY("shid"),
	FOREIGN KEY("pfid") REFERENCES "Playfields"("pfid"),
	FOREIGN KEY("shipid") REFERENCES "Entities"("entityid"),
	FOREIGN KEY("stationid") REFERENCES "Entities"("entityid"),
	FOREIGN KEY("playerid") REFERENCES "Entities"("entityid")
);
DROP INDEX IF EXISTS "ss_idx_name";
CREATE UNIQUE INDEX IF NOT EXISTS "ss_idx_name" ON "SolarSystems" (
	"name"
);
DROP INDEX IF EXISTS "pf_idx_name";
CREATE UNIQUE INDEX IF NOT EXISTS "pf_idx_name" ON "Playfields" (
	"name"
);
DROP INDEX IF EXISTS "pii_idx_piid";
CREATE INDEX IF NOT EXISTS "pii_idx_piid" ON "PlayerInventoryItems" (
	"piid"
);
DROP INDEX IF EXISTS "lolo_idx_piid";
CREATE INDEX IF NOT EXISTS "lolo_idx_piid" ON "LoginLogoff" (
	"entityid",
	"playerid",
	"clientid",
	"loginticks"
);
DROP INDEX IF EXISTS "ent_fac_pfid2";
CREATE INDEX IF NOT EXISTS "ent_fac_pfid2" ON "Entities" (
	"isstructure",
	"isremoved",
	"facgroup",
	"facid"
);
DROP INDEX IF EXISTS "ent_pfid_struct_rem";
CREATE INDEX IF NOT EXISTS "ent_pfid_struct_rem" ON "Entities" (
	'pfid',
	'isstructure',
	'isremoved'
);
DROP INDEX IF EXISTS "discpois_pfid";
CREATE INDEX IF NOT EXISTS "discpois_pfid" ON "DiscoveredPOIs" (
	'pfid'
);
DROP INDEX IF EXISTS "discpois_poiid";
CREATE INDEX IF NOT EXISTS "discpois_poiid" ON "DiscoveredPOIs" (
	"poiid"
);
DROP INDEX IF EXISTS "discpois_poitype_pfid_poiid_fac";
CREATE INDEX IF NOT EXISTS "discpois_poitype_pfid_poiid_fac" ON "DiscoveredPOIs" (
	"poitype",
	"pfid",
	"poiid",
	"facgroup",
	"facid"
);
DROP INDEX IF EXISTS "structs_teleporter";
CREATE INDEX IF NOT EXISTS "structs_teleporter" ON "Structures" (
	"hasteleporter"
);
DROP INDEX IF EXISTS "chm_chn_rece";
CREATE INDEX IF NOT EXISTS "chm_chn_rece" ON "ChatMessages" (
	"channel",
	"recentityid"
);
DROP INDEX IF EXISTS "chm_chn_fac";
CREATE INDEX IF NOT EXISTS "chm_chn_fac" ON "ChatMessages" (
	"channel",
	"recfacgroup",
	"recfacid"
);
DROP INDEX IF EXISTS "structs_splocked";
CREATE INDEX IF NOT EXISTS "structs_splocked" ON "Structures" (
	"hassplocked"
);
DROP INDEX IF EXISTS "structs_spunlocked";
CREATE INDEX IF NOT EXISTS "structs_spunlocked" ON "Structures" (
	"hasspunlocked"
);
DROP INDEX IF EXISTS "discpois_pfid_eid_facgr_poitype";
CREATE INDEX IF NOT EXISTS "discpois_pfid_eid_facgr_poitype" ON "DiscoveredPOIs" (
	"poitype",
	"facgroup",
	"entityid",
	"pfid"
);
DROP INDEX IF EXISTS "pf_idx_ssid2";
CREATE INDEX IF NOT EXISTS "pf_idx_ssid2" ON "Playfields" (
	"ssid"
);
DROP VIEW IF EXISTS "ChangedPlayfiedsInShip";
CREATE VIEW ChangedPlayfiedsInShip  (entityid, shipid, shipname, frompf, topf, distanceau, gametime)AS SELECT cp.entityid, e.entityid, e.name, p1.name, p2.name,        sqrt((p1.sectorx - p2.sectorx) * (p1.sectorx - p2.sectorx) + (p1.sectory - p2.sectory) * (p1.sectory - p2.sectory) + (p1.sectorz - p2.sectorz) * (p1.sectorz - p2.sectorz))*0.1,        cp.gametime FROM ChangedPlayfields cp, Playfields p1, Playfields p2, Entities e WHERE cp.frompfid = p1.pfid AND cp.topfid = p2.pfid AND e.entityid = cp.attentityid ORDER BY cp.gametime DESC;
COMMIT;
