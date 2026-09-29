Feature: Recipe comments
    As an authorized user I want to write and manage comments on a recipe detail page

    Scenario: Authorized user writes a comment on recipe details
        Given There is a public recipe "Pasta Carbonara" authored by "alice"
        And I am logged in as "bob"
        When I open the recipe "Pasta Carbonara" from the recipes page
        And I write a comment "Looks delicious"
        Then I should see the comment "Looks delicious" by "bob" as the newest comment

    Scenario: Guest cannot write a comment
        Given There is a public recipe "Pasta Carbonara" authored by "alice"
        And I am not logged in
        When I open the recipe "Pasta Carbonara" from the recipes page
        Then I should not be able to write a comment

    Scenario: Recipe author deletes a comment
        Given There is a public recipe "Pasta Carbonara" authored by "alice"
        And The recipe has a comment "Looks delicious" by "bob"
        And I am logged in as "alice"
        When I open the recipe "Pasta Carbonara" from the recipes page
        And I delete the comment "Looks delicious" by "bob"
        Then I should not see the comment "Looks delicious" by "bob"

    Scenario: Non-author cannot delete a comment
        Given There is a public recipe "Pasta Carbonara" authored by "alice"
        And The recipe has a comment "Family favorite" by "alice"
        And I am logged in as "bob"
        When I open the recipe "Pasta Carbonara" from the recipes page
        Then I should not be able to delete comments
