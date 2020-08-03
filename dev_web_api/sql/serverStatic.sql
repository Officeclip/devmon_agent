BEGIN TRANSACTION;

INSERT INTO 
    "groups" 
    ("id","server_guid") 
VALUES 
    (1,'ad137570-68c8-481c-ad22-f96e3cf41ea5');

INSERT INTO 
    "users" 
    ("user_id","email_address", "password", "email_opt") 
VALUES 
    (1,'skd@officeclip.com', 'abcd1234@A', 1);

INSERT INTO "monitorCommands" ("monitor_command_id", "name", "org_id", "type", "arg1", "arg2", "unit") VALUES ('1', 'OfficeClip Ping', '1', 'url.ping', 'https://www.officeclip.com', '', 'ms');
INSERT INTO "monitorCommands" ("monitor_command_id", "name", "org_id", "type", "arg1", "arg2", "unit") VALUES ('2', 'Google Ping', '1', 'url.ping', 'https://www.google.com', '', 'ms');
INSERT INTO "monitorCommands" ("monitor_command_id", "name", "org_id", "type", "arg1", "arg2", "unit") VALUES ('3', 'CPU %', '1', 'cpu.percent', '', '', '%');
INSERT INTO "monitorCommands" ("monitor_command_id", "name", "org_id", "type", "arg1", "arg2", "unit") VALUES ('4', 'Memory Free', '1', 'memory.free', '', '', 'gb');
INSERT INTO "monitorCommands" ("monitor_command_id", "name", "org_id", "type", "arg1", "arg2", "unit") VALUES ('5', 'C: Free', '1', 'drive.free', 'C:\', '', 'gb');
INSERT INTO "monitorCommands" ("monitor_command_id", "name", "org_id", "type", "arg1", "arg2", "unit") VALUES ('6', 'D: Free', '1', 'drive.free', 'D:\', '', 'gb');
INSERT INTO "monitorCommands" ("monitor_command_id", "name", "org_id", "type", "arg1", "arg2", "unit") VALUES ('7', 'Idle', '1', 'os.idletime', '', '', 'mins');

INSERT INTO "monitorCommandLimits" ("type", "org_id", "warning_limit", "error_limit", "is_low_limit") VALUES ('url.ping', '1', '500.0', '1000.0', '0');
INSERT INTO "monitorCommandLimits" ("type", "org_id", "warning_limit", "error_limit", "is_low_limit") VALUES ('cpu.percent', '1', '50.0', '75.0', '0');
INSERT INTO "monitorCommandLimits" ("type", "org_id", "warning_limit", "error_limit", "is_low_limit") VALUES ('memory.free', '1', '2.0', '1.0', '1');
INSERT INTO "monitorCommandLimits" ("type", "org_id", "warning_limit", "error_limit", "is_low_limit") VALUES ('drive.free', '1', '20.0', '10.0', '1');

COMMIT;