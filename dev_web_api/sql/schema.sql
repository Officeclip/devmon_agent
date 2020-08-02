BEGIN TRANSACTION;
DROP TABLE IF EXISTS "users";
CREATE TABLE IF NOT EXISTS "users" (
	"user_id"	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
	"email_address"	TEXT NOT NULL,
	"password" TEXT NOT NULL
	"email_opt" INTEGER 
);
DROP TABLE IF EXISTS "userNotifications";
CREATE TABLE IF NOT EXISTS "userNotifications" (
	"user_id"	INTEGER NOT NULL,
	"agent_id"	INTEGER NOT NULL,
	"monitor_command_id"	INTEGER,
	"last_notified"	TEXT
);
DROP TABLE IF EXISTS "agents";
CREATE TABLE IF NOT EXISTS "agents" (
	"agent_id"	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
	"guid"	TEXT NOT NULL UNIQUE,
	"machine_name"	TEXT NOT NULL,
	"org_id"	INTEGER NOT NULL,
	"registration_date"	TEXT,
	"last_queried"	TEXT,
	"last_reply_received"	TEXT,
	"alias"	TEXT,
	"enabled"	INTEGER NOT NULL DEFAULT 1
);
DROP TABLE IF EXISTS "groups";
CREATE TABLE IF NOT EXISTS "groups" (
	"id"	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
	"server_guid"	TEXT NOT NULL
);
DROP TABLE IF EXISTS "agentResources";
CREATE TABLE IF NOT EXISTS "agentResources" (
	"agent_id"	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
	"stable_device_json"	TEXT,
	"last_updated_date"	TEXT,
	FOREIGN KEY("agent_id") REFERENCES "agents"("agent_id")
);
DROP TABLE IF EXISTS "monitorValues";
CREATE TABLE IF NOT EXISTS "monitorValues" (
	"agent_id"	INTEGER NOT NULL,
	"monitor_command_id"	INTEGER NOT NULL,
	"value"	REAL NOT NULL,
	"return_code"	INTEGER,
	"error_message"	TEXT,
	FOREIGN KEY("agent_id") REFERENCES "agents"("agent_id")
);
DROP TABLE IF EXISTS "monitorCommands";
CREATE TABLE IF NOT EXISTS "monitorCommands" (
	"monitor_command_id"	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
	"name"	TEXT NOT NULL,
	"org_id"	INTEGER NOT NULL DEFAULT 1,
	"type"	TEXT NOT NULL,
	"arg1"	TEXT,
	"arg2"	TEXT,
	"unit"  TEXT
);
DROP TABLE IF EXISTS "monitorCommandLimits";
CREATE TABLE IF NOT EXISTS "monitorCommandLimits" (
	"type"	TEXT NOT NULL,
	"org_id"	INTEGER NOT NULL,
	"warning_limit"	REAL,
	"error_limit"	REAL,
	"is_low_limit"	INTEGER
);
DROP TABLE IF EXISTS "history";
CREATE TABLE IF NOT EXISTS "history" (
	"frequency"	INTEGER NOT NULL DEFAULT 0,
	"agent_id"	INTEGER NOT NULL,
	"monitor_command_id"	INTEGER NOT NULL,
	"date"	TEXT NOT NULL,
	"value"	INTEGER NOT NULL
);

DROP TABLE IF EXISTS "agent_group_agent"; 
CREATE TABLE "agent_group_agent" (
	"agent_group_id"	INTEGER NOT NULL,
	"agent_id"	INTEGER NOT NULL
);

DROP TABLE IF EXISTS "agent_groups";
CREATE TABLE "agent_groups" (
	"agent_group_id"	INTEGER NOT NULL UNIQUE,
	"agent_group_name"	TEXT NOT NULL,
	"org_id"	INTEGER NOT NULL,
	PRIMARY KEY("agent_group_id" AUTOINCREMENT)
);

DROP INDEX IF EXISTS "agent-guid";
CREATE UNIQUE INDEX IF NOT EXISTS "agent-guid" ON "agents" (
	"guid"
);
DROP INDEX IF EXISTS "agentResources_agentId";
CREATE UNIQUE INDEX IF NOT EXISTS "agentResources_agentId" ON "agentResources" (
	"agent_id"
);
DROP INDEX IF EXISTS "monitorValues_agentId_monitorCommandId";
CREATE INDEX IF NOT EXISTS "monitorValues_agentId_monitorCommandId" ON "monitorValues" (
	"agent_id",
	"monitor_command_id"
);

COMMIT;
