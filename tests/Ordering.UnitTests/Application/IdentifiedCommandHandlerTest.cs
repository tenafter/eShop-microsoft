using System;
using System.Threading;
using System.Threading.Tasks;
using eShop.Ordering.Application.Commands;
using eShop.Ordering.Infrastructure.Idempotency;
using MediatR;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace eShop.Ordering.UnitTests.Application;
/// <summary>
/// 식별된 명령 핸들러에 대한 단위 테스트를 포함합니다.
/// </summary>

[TestClass]
public class IdentifiedCommandHandlerTest
{
    private readonly IRequestManager _requestManager;
    private readonly IMediator _mediator;
    private readonly ILogger<IdentifiedCommandHandler<CreateOrderCommand, bool>> _loggerMock;

    public IdentifiedCommandHandlerTest()
    {
        _requestManager = Substitute.For<IRequestManager>();
        _mediator = Substitute.For<IMediator>();
        _loggerMock = Substitute.For<ILogger<IdentifiedCommandHandler<CreateOrderCommand, bool>>>();
    }

    [TestMethod]
    /// <summary>
    /// 주문이 존재하지 않을 때 핸들러가 명령을 전송하는지 테스트합니다.
    /// </summary>
    /// <returns>비동기 작업을 나타내는 Task</returns>
    public async Task Handler_sends_command_when_order_no_exists()
    {
        // Arrange (테스트 준비)
        // 새로운 GUID를 생성하여 주문을 식별하는 고유 ID로 사용
        var fakeGuid = Guid.NewGuid();
        // FakeOrderRequest()로 가짜 주문 데이터를 생성하고 IdentifiedCommand로 감싸서 명령 객체 생성
        var fakeOrderCmd = new IdentifiedCommand<CreateOrderCommand, bool>(FakeOrderRequest(), fakeGuid);

        // _requestManager가 주문이 존재하는지 확인할 때 false를 반환하도록 설정 
        // (즉, 주문이 아직 존재하지 않음을 시뮬레이션)
        _requestManager.ExistAsync(Arg.Any<Guid>())
            .Returns(Task.FromResult(false));

        // _mediator가 명령을 처리할 때 성공(true)을 반환하도록 설정
        _mediator.Send(Arg.Any<IRequest<bool>>(), default)
            .Returns(Task.FromResult(true));

        // Act (테스트 실행)
        // 실제 핸들러 객체를 생성하고 가짜 주문 명령을 처리
        var handler = new CreateOrderIdentifiedCommandHandler(_mediator, _requestManager, _loggerMock);
        var result = await handler.Handle(fakeOrderCmd, CancellationToken.None);

        // Assert (결과 검증)
        // 핸들러가 true를 반환했는지 확인
        Assert.IsTrue(result);
        // _mediator.Send가 정확히 한 번 호출되었는지 확인 
        // (새로운 주문이므로 실제로 명령이 처리되어야 함)
        await _mediator.Received().Send(Arg.Any<IRequest<bool>>(), default);
    }

    [TestMethod]
    /// <summary>
    /// 이미 존재하는 주문일 경우 핸들러가 명령을 보내지 않는지 테스트합니다.
    /// </summary>
    /// <returns>비동기 작업을 나타내는 Task</returns>
    public async Task Handler_sends_no_command_when_order_already_exists()
    {
        // Arrange (테스트 준비)
        // 새로운 GUID를 생성하여 주문을 식별하는 고유 ID로 사용
        var fakeGuid = Guid.NewGuid();
        // FakeOrderRequest()로 가짜 주문 데이터를 생성하고 IdentifiedCommand로 감싸서 명령 객체 생성
        var fakeOrderCmd = new IdentifiedCommand<CreateOrderCommand, bool>(FakeOrderRequest(), fakeGuid);

        // _requestManager가 주문이 존재하는지 확인할 때 true를 반환하도록 설정
        // (즉, 주문이 이미 존재함을 시뮬레이션)
        _requestManager.ExistAsync(Arg.Any<Guid>())
            .Returns(Task.FromResult(true));

        // _mediator가 명령을 처리할 때 성공(true)을 반환하도록 설정
        _mediator.Send(Arg.Any<IRequest<bool>>(), default)
            .Returns(Task.FromResult(true));

        // Act (테스트 실행)
        // 실제 핸들러 객체를 생성하고 가짜 주문 명령을 처리
        var handler = new CreateOrderIdentifiedCommandHandler(_mediator, _requestManager, _loggerMock);
        var result = await handler.Handle(fakeOrderCmd, CancellationToken.None);

        // Assert (결과 검증)
        // _mediator.Send가 호출되지 않았는지 확인
        // (이미 존재하는 주문이므로 명령이 처리되지 않아야 함)
        await _mediator.DidNotReceive().Send(Arg.Any<IRequest<bool>>(), default);
    }

    private CreateOrderCommand FakeOrderRequest(Dictionary<string, object> args = null)
    {
        return new CreateOrderCommand(
            new List<BasketItem>(),
            userId: args != null && args.ContainsKey("userId") ? (string)args["userId"] : null,
            userName: args != null && args.ContainsKey("userName") ? (string)args["userName"] : null,
            city: args != null && args.ContainsKey("city") ? (string)args["city"] : null,
            street: args != null && args.ContainsKey("street") ? (string)args["street"] : null,
            state: args != null && args.ContainsKey("state") ? (string)args["state"] : null,
            country: args != null && args.ContainsKey("country") ? (string)args["country"] : null,
            zipcode: args != null && args.ContainsKey("zipcode") ? (string)args["zipcode"] : null,
            cardNumber: args != null && args.ContainsKey("cardNumber") ? (string)args["cardNumber"] : "1234",
            cardExpiration: args != null && args.ContainsKey("cardExpiration") ? (DateTime)args["cardExpiration"] : DateTime.MinValue,
            cardSecurityNumber: args != null && args.ContainsKey("cardSecurityNumber") ? (string)args["cardSecurityNumber"] : "123",
            cardHolderName: args != null && args.ContainsKey("cardHolderName") ? (string)args["cardHolderName"] : "XXX",
            cardTypeId: args != null && args.ContainsKey("cardTypeId") ? (int)args["cardTypeId"] : 0);
    }
}
