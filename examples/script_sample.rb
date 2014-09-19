module L2Apf
	@api = Server::Script::Api.new

	servers = @api.login("test", "123456")
	characters = @api.select_server(servers.first)
	@api.select_player(characters.select{|c| c.name == "test"}.first)
	
	@api.chat_message do |channel, message, from, author|
		print message
	end

	while @api.state != Server::Script::State.NotConnected do
		System::Threading::Monitor.Enter(@api.sync)
		@api.do_events()
		@api.wait(lambda{|r| true})
		System::Threading::Monitor.Exit(@api.sync)
	end
end