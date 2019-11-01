using KennelAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TestKennelAPI
{
    public class AnimalTests
    {
        [Fact]
        public async Task Test_Create_Animal()
        {
            /*
                Generate a new instance of an HttpClient that you can
                use to generate HTTP requests to your API controllers.
                The `using` keyword will automatically dispose of this
                instance of HttpClient once your code is done executing.
            */
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */

                // Construct a new student object to be sent to the API
                Animal jack = new Animal
                {
                    Name = "Jack",
                    Breed = "Cocker Spaniel",
                    Age = 4,
                    HasShots = true
                };

                // Serialize the C# object into a JSON string
                var jackAsJSON = JsonConvert.SerializeObject(jack);


                /*
                    ACT
                */

                // Use the client to send the request and store the response
                var response = await client.PostAsync(
                    "/api/animals",
                    new StringContent(jackAsJSON, Encoding.UTF8, "application/json")
                );

                // Store the JSON body of the response
                string responseBody = await response.Content.ReadAsStringAsync();

                // Deserialize the JSON into an instance of Animal
                var newJack = JsonConvert.DeserializeObject<Animal>(responseBody);


                /*
                    ASSERT
                */

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal("Jack", newJack.Name);
                Assert.Equal("Cocker Spaniel", newJack.Breed);
                Assert.Equal(4, newJack.Age);
            }
        }

        [Fact]
        public async Task Test_Get_All_Animals()
        {

            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var response = await client.GetAsync("/api/animals");


                string responseBody = await response.Content.ReadAsStringAsync();
                var animalList = JsonConvert.DeserializeObject<List<Animal>>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(animalList.Count > 0);
            }
        }

        [Fact]
        public async Task Test_Modify_Animal()
        {
            // New last name to change to and test
            int newAge = 5;

            using (var client = new APIClientProvider().Client)
            {
                /*
                    PUT section
                */
                Animal modifiedJack = new Animal
                {
                    Name = "Jack",
                    Breed = "Cocker Spaniel",
                    Age = newAge,
                    HasShots = true
                };
                var modifiedJackAsJSON = JsonConvert.SerializeObject(modifiedJack);

                var response = await client.PutAsync(
                    "/api/animals/1",
                    new StringContent(modifiedJackAsJSON, Encoding.UTF8, "application/json")
                );
                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);


                /*
                    GET section
                    Verify that the PUT operation was successful
                */
                var getJack = await client.GetAsync("/api/animals/1");
                getJack.EnsureSuccessStatusCode();

                string getJackBody = await getJack.Content.ReadAsStringAsync();
                Animal newJack = JsonConvert.DeserializeObject<Animal>(getJackBody);

                Assert.Equal(HttpStatusCode.OK, getJack.StatusCode);
                Assert.Equal(newAge, newJack.Age);
            }
        }
    }
}
