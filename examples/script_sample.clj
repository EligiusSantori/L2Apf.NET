(assembly-load-from "core.dll")
(assembly-load-from "script.dll")

(import L2Apf.Server.Script.Result.Result)
(import L2Apf.Server.Script.State)
(import L2Apf.Server.Script.Api)

(def api (new Api))

; setup event handlers
(. api add_ChatMessage (fn [channel message from author] (println message)))

; log in to game
(. api SelectPlayer (first (filter
	(fn [c] (= (. c Name) "test"))
	(. api SelectServer (first
		(. api Login "test" "123456")
	))
)))

; TODO lock macro!

; main loop
(while (not= (. api State) State/NotConnected) (do
	(. System.Threading.Monitor Enter (. api Sync))
	(. api DoEvents)
	(. api Wait (fn [r] true) nil)
	(. System.Threading.Monitor Exit (. api Sync))
))
