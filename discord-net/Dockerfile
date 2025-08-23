# Use a smaller runtime image as the base
# FROM --platform=linux/arm64 mcr.microsoft.com/dotnet/runtime:8.0 AS final
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS final

# Set the working directory inside the container
WORKDIR /discord-bot

# Copy the pre-built application from the host into the container
# This path is relative to the build context, which is '/YourConsoleApp' if built from there.
COPY ./publish ./
# COPY ./efbundle ./efbundle
# RUN chmod +x ./efbundle

# Setting Locale and Encoding
ENV LC_ALL=en_US.UTF-8 \
    LANG=en_US.UTF-8

# Define the entry point for the application
# ENTRYPOINT ["sh", "-c", "./efbundle && dotnet RuTakingTooLong.dll"]
ENTRYPOINT ["dotnet", "RuTakingTooLong.dll"]