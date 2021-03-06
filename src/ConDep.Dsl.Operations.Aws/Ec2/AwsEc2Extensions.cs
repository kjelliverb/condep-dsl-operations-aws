﻿using System;
using ConDep.Dsl.Operations.Aws;
using ConDep.Dsl.Operations.Aws.Ec2;
using ConDep.Dsl.Operations.Aws.Ec2.Builders;
using ConDep.Dsl.Operations.Aws.Ec2.Model;
using ConDep.Dsl.Operations.Aws.Ec2.Terminate;

namespace ConDep.Dsl
{
    /// <summary>
    /// Usuefult things:
    /// 1) ImageId (Mandatory, ConDep could select default)
    /// 2) InstanceType (t2.micro etc) (Mandatory, ConDep could select default)
    /// 3) Count (min count, max count) (Default 1)
    /// 4) KeyName / KeyPair (public key) (for encrypting win password) (Mandatory)
    /// 5) Private Key (pem) (for decrypting win password) (Mandatory)
    /// 6) UserData (Optional)
    /// 7) Network Interface (new or id) (Optional?)
    ///     7.1) PublicIP?
    ///     7.2) Index
    ///     7.3) SubnetId
    ///     7.4) Security Groups
    ///     7.5) DeleteOnTermination?
    ///     7.6) Private ip(s)
    /// 8) Instance Initiated Shutdown behaviour (stops or terminates) (default stop)
    /// 9) Monitor?
    /// 10) Placement (availability zone)
    /// 11) Private IP
    /// 12) Subnetid (if not on interface??)
    /// 13) Client Token (for idempotencies and termination/description later on)
    /// 14) Block Device Mappings (Disks) (Instance Store Volumes - host volume / EBS - Remote Storage Device)
    ///     If set, will override mapping defined in AMI (image)
    ///     14.1) Device Name (e.g. /dev/sdh)
    ///     14.2) EBS settings
    ///     14.3) NoDevice - Exclude from AMI
    ///     14.4) VirtualName (e.g. ephemeral0) 
    /// </summary>
    public static class AwsEc2Extensions
    {
        /// <summary>
        /// Bootstrap one or more Amazon AWS Virtual Private Cloud (VPC) instances and adds 
        /// them to ConDep's servers collection. This method assume mandatory settings are 
        /// found in ConDep's environment file. If you prefer to specify settings directly, use
        /// the other overload and specify settings in code. 
        /// </summary>
        /// <param name="bootstrap"></param>
        /// <param name="bootstrapId">Unique, case-sensitive identifier you provide to ensure the idempotency of the bootstrap operation. 
        /// In AWS this is refered to as the Client Token.</param>
        /// <returns></returns>
        public static IOfferAwsOperations CreateInstances(this IOfferAwsEc2Operations ec2, string bootstrapId)
        {
            var ec2Builder = ec2 as AwsEc2OperationsBuilder;
            var awsOpsBuilder = ec2Builder.AwsOperations as AwsOperationsBuilder;

            var options = new AwsBootstrapOptionsValues(bootstrapId);
            var awsBootstrapOperation = new AwsBootstrapOperation(options);

            Configure.Operation(awsOpsBuilder.LocalOperations, awsBootstrapOperation);
            return ec2Builder.AwsOperations;
        }

        /// <summary>
        /// Bootstrap one or more Amazon AWS Virtual Private Cloud (VPC) instances and adds 
        /// them to ConDep's servers collection. This method assume mandatory settings are 
        /// found in ConDep's environment file. If you prefer to specify settings directly, use
        /// the other overload and specify settings in code.
        /// </summary>
        /// <param name="ec2"></param>
        /// <param name="tags">Tags to use as identifiers to ensure the idempotency of the bootstrap operation. 
        /// Make sure that the tag(s) used uniquely identifies instances you want to manage by your ConDep operation.</param>
        /// <returns></returns>
        public static IOfferAwsOperations CreateInstances(this IOfferAwsEc2Operations ec2, Action<IOfferAwsTagOptions> tags)
        {            
            var ec2Builder = ec2 as AwsEc2OperationsBuilder;
            var awsOpsBuilder = ec2Builder.AwsOperations as AwsOperationsBuilder;

            var instanceOptions = new AwsBootstrapOptionsBuilder(tags);
            var awsBootstrapOperation = new AwsBootstrapOperation(instanceOptions.Values);

            Configure.Operation(awsOpsBuilder.LocalOperations, awsBootstrapOperation);
            return ec2Builder.AwsOperations;
        }

        /// <summary>
        /// Bootstrap one or more Amazon AWS Virtual Private Cloud (VPC) instances and adds 
        /// them to ConDep's servers collection. This method assume mandatory settings not  
        /// specified in code is found in ConDep's environment file. 
        /// </summary>
        /// <param name="bootstrap"></param>
        /// <param name="bootstrapId">Unique, case-sensitive identifier you provide to ensure the idempotency of the bootstrap operation.</param>
        /// <param name="options">Additional configuration options for bootstrapping instances.</param>
        /// <returns></returns>
        public static IOfferAwsOperations CreateInstances(this IOfferAwsEc2Operations ec2, string bootstrapId, Action<IOfferAwsBootstrapOptions> options)
        {
            var ec2Builder = ec2 as AwsEc2OperationsBuilder;
            var awsOpsBuilder = ec2Builder.AwsOperations as AwsOperationsBuilder;

            var instanceOptions = new AwsBootstrapOptionsBuilder(bootstrapId);
            options(instanceOptions);

            var awsBootstrapOperation = new AwsBootstrapOperation(instanceOptions.Values);
            Configure.Operation(awsOpsBuilder.LocalOperations, awsBootstrapOperation);
            return ec2Builder.AwsOperations;
        }

        /// <summary>
        /// Bootstrap one or more Amazon AWS Virtual Private Cloud (VPC) instances and adds 
        /// them to ConDep's servers collection. This method assume mandatory settings not  
        /// specified in code is found in ConDep's environment file. 
        /// 
        /// </summary>
        /// <param name="ec2"></param>
        /// <param name="instanceTags"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IOfferAwsOperations CreateInstances(this IOfferAwsEc2Operations ec2, Action<IOfferAwsTagOptions> tags, Action<IOfferAwsBootstrapOptions> options)
        {
            var ec2Builder = ec2 as AwsEc2OperationsBuilder;
            var awsOpsBuilder = ec2Builder.AwsOperations as AwsOperationsBuilder;

            var instanceOptions = new AwsBootstrapOptionsBuilder(tags);
            options(instanceOptions);

            var awsBootstrapOperation = new AwsBootstrapOperation(instanceOptions.Values);
            Configure.Operation(awsOpsBuilder.LocalOperations, awsBootstrapOperation);
            return ec2Builder.AwsOperations;
        }

        /// <summary>
        /// Terminates the instances that are bootstrapped with the given bootstrap-ID.
        /// </summary>
        /// <param name="ec2"></param>
        /// <param name="bootstrapId">Unique, case-sensitive identifier you provide to ensure the idempotency of the bootstrap operation. 
        /// In AWS this is refered to as the Client Token.</param>
        /// <returns></returns>
        public static IOfferAwsOperations TerminateInstances(this IOfferAwsEc2Operations ec2, string bootstrapId)
        {
            var ec2Builder = ec2 as AwsEc2OperationsBuilder;
            var awsOpsBuilder = ec2Builder.AwsOperations as AwsOperationsBuilder;

            var options = new AwsTerminateOptionsValues(bootstrapId);
            var awsTerminateOperation = new AwsTerminateOperation(options);

            Configure.Operation(awsOpsBuilder.LocalOperations, awsTerminateOperation);
            return ec2Builder.AwsOperations;
        }

        public static IOfferAwsOperations StopInstances(this IOfferAwsEc2Operations ec2)
        {
            throw new NotImplementedException();
        }

        public static IOfferAwsOperations StartInstances(this IOfferAwsEc2Operations ec2)
        {
            throw new NotImplementedException();
        }

        public static IOfferAwsOperations CreateImage(this IOfferAwsEc2Operations ec2)
        {
            throw new NotImplementedException();
        }

        public static IOfferAwsOperations DeleteImage(this IOfferAwsEc2Operations ec2)
        {
            throw new NotImplementedException();
        }

        public static IOfferAwsOperations CreateKeyPair(this IOfferAwsEc2Operations ec2, string name)
        {
            throw new NotImplementedException();
        }

        public static IOfferAwsOperations ImportKeyPair(this IOfferAwsEc2Operations ec2, string name, string publicKey)
        {
            throw new NotImplementedException();
        }

        public static IOfferAwsOperations DeleteKeyPair(this IOfferAwsEc2Operations ec2)
        {
            throw new NotImplementedException();
        }

        public static IOfferAwsOperations CreateSnapshot(this IOfferAwsEc2Operations ec2)
        {
            throw new NotImplementedException();
        }

        public static IOfferAwsOperations DeleteSnapshot(this IOfferAwsEc2Operations ec2)
        {
            throw new NotImplementedException();
        }
    }
}