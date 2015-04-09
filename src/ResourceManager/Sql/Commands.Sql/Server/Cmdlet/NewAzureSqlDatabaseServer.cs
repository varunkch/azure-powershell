﻿// ----------------------------------------------------------------------------------
//
// Copyright Microsoft Corporation
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Hyak.Common;
using Microsoft.Azure.Commands.Sql.Properties;

namespace Microsoft.Azure.Commands.Sql.Server.Cmdlet
{
    /// <summary>
    /// Defines the Get-AzureSqlDatabaseServer cmdlet
    /// </summary>
    [Cmdlet(VerbsCommon.New, "AzureSqlDatabaseServer",
        ConfirmImpact = ConfirmImpact.Low)]
    public class NewAzureSqlDatabaseServer : AzureSqlDatabaseServerCmdletBase
    {
        /// <summary>
        /// Gets or sets the name of the database server to use.
        /// </summary>
        [Parameter(Mandatory = true, 
            HelpMessage = "SQL Database server name.")]
        [ValidateNotNullOrEmpty]
        public string ServerName { get; set; }

        /// <summary>
        /// The SQL administrator credentials for the server
        /// </summary>
        [Parameter(Mandatory = true,
            HelpMessage = "The SQL administrator credentials for the server")]
        [ValidateNotNull]
        public PSCredential SqlAdminCredentials { get; set; }

        /// <summary>
        /// The location in which to create the server
        /// </summary>
        [Parameter(Mandatory = true,
            HelpMessage = "The location in which to create the server")]
        [ValidateNotNullOrEmpty]
        public string Location { get; set; }

        /// <summary>
        /// The tags to associate with the Azure Sql Database Server
        /// </summary>
        [Parameter(Mandatory = false,
            HelpMessage = "The tags to associate with the Azure Sql Database Server")]
        public Dictionary<string, string> Tags { get; set; }

        /// <summary>
        /// Gets or sets the server version
        /// </summary>
        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "Determines which version of Sql Azure server is created")]
        [ValidateNotNullOrEmpty]
        public string ServerVersion { get; set; }

        /// <summary>
        /// Check to see if the server already exists in this resource group.
        /// </summary>
        /// <returns>Null if the server doesn't exist.  Otherwise throws exception</returns>
        protected override IEnumerable<Model.AzureSqlDatabaseServerModel> GetEntity()
        {
            try
            {
                ModelAdapter.GetServer(this.ResourceGroupName, this.ServerName);
            }
            catch(CloudException ex)
            {
                if(ex.Response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    // This is what we want.  We looked and there is no server with this name.
                    return null;
                }

                // Unexpected exception encountered
                throw;
            }

            // The server already exists
            throw new PSArgumentException(
                string.Format(Resources.ServerNameExists, this.ServerName),
                "ServerName");
        }

        /// <summary>
        /// Generates the model from user input.
        /// </summary>
        /// <param name="model">This is null since the server doesn't exist yet</param>
        /// <returns>The generated model from user input</returns>
        protected override IEnumerable<Model.AzureSqlDatabaseServerModel> ApplyUserInputToModel(IEnumerable<Model.AzureSqlDatabaseServerModel> model)
        {
            List<Model.AzureSqlDatabaseServerModel> newEntity = new List<Model.AzureSqlDatabaseServerModel>();
            newEntity.Add(new Model.AzureSqlDatabaseServerModel()
                {
                    Location = this.Location,
                    ResourceGroupName = this.ResourceGroupName,
                    ServerName = this.ServerName,
                    ServerVersion = this.ServerVersion,
                    SqlAdminPassword = this.SqlAdminCredentials.Password,
                    SqlAdminUserName = this.SqlAdminCredentials.UserName,
                    Tags = this.Tags
                });
            return newEntity;
        }

        /// <summary>
        /// Sends the changes to the service -> Creates the server
        /// </summary>
        /// <param name="entity">The server to create</param>
        /// <returns>The created server</returns>
        protected override IEnumerable<Model.AzureSqlDatabaseServerModel> PersistChanges(IEnumerable<Model.AzureSqlDatabaseServerModel> entity)
        {
            return new List<Model.AzureSqlDatabaseServerModel>() { 
                ModelAdapter.UpsertServer(entity.First()) 
            };
        }
    }
}
