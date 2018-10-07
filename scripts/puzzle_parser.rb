require 'bundler/inline'

gemfile do
  source 'https://rubygems.org'
  gem 'json', require: true
	gem 'pry'
	gem 'pry-byebug'
	gem 'fast_blank'
end

require 'json'

file = File.open("faq.txt", "r")
contents = file.read
file.close

contents_lines = contents.split("\n")
puzzle_lines = contents_lines[663..49057]

puzzle_lines.reject! { |line| line.blank? }
puzzle_lines.reject! { |line| /^\s\s\s/ =~ line }
puzzle_lines.reject! { |line| /^-+\r/ =~ line }

puzzles = []
current_puzzle = []

puzzle_lines.each do |puzzle_line|
	start_of_puzzle = /Rotations/ =~ puzzle_line || /-=-+/ =~ puzzle_line

	if start_of_puzzle
		if current_puzzle.size > 0
			puzzles << current_puzzle
			current_puzzle = []
		else
			current_puzzle = []
		end
	end

	current_puzzle << puzzle_line
end

binding.pry
puts 'hi'